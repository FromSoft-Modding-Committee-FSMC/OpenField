using System;
using System.Collections.Generic;

using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

using OFC.Utility;

namespace OFC.Input
{
    public static class InputManager
    {
        private static NativeWindow glfwWindow = null;

        private static Dictionary<string, InputMapping> inputMaps;
        private static Dictionary<string, InputState> inputStates;

        public static void Initialize(NativeWindow nativeWindow)
        {
            glfwWindow = nativeWindow;
            glfwWindow.JoystickConnected += OnJoystickConnectDisconnect;

            inputMaps = new Dictionary<string, InputMapping>
            {
                { "TestButton", new InputMapping
                    {
                        controls = new Control[]
                        {
                            Control.Button(InputDeviceType.Gamepad, Input.GamepadStart),
                            Control.Button(InputDeviceType.Mouse, Input.MouseMiddle),
                            Control.Button(InputDeviceType.Keyboard, Input.KeyboardEscape)
                        },
                        mappingType = InputMappingType.Button
                    }
                },
                { "TestAxis", new InputMapping
                    {
                        controls = new Control[]
                        {
                            Control.Axis(InputDeviceType.Gamepad, Input.GamepadAxisLX, 1f),
                            Control.Axis(InputDeviceType.Mouse, Input.MouseAxisX, 1f),
                            Control.Axis(InputDeviceType.Keyboard, Input.KeyboardA, Input.KeyboardD, 1f)
                        },
                        mappingType = InputMappingType.Axis,
                    }
                }
            };

            inputStates = new Dictionary<string, InputState>();
            foreach(string key in inputMaps.Keys)
            {
                inputStates[key] = new InputState();
            }
        }

        public static void Update()
        {
            if(glfwWindow == null)
            {
                Log.Error("No window assigned to input manager!");
                return;
            }

            //Poll each device
            foreach(string key in inputMaps.Keys)
            {
                InputMapping map = inputMaps[key];

                //Capture the current state
                InputState state = inputStates[key];
                state.lastValue = state.value;
                state.value = 0f;

                if(map.mappingType == InputMappingType.Button)
                {
                    bool buttonState = false;
                    foreach(Control control in map.controls)
                    {
                        Input[] controlInputs = control.inputs;

                        switch(control.deviceType)
                        {
                            case InputDeviceType.Mouse:
                                buttonState |= PollMouseButtonState(ref controlInputs);
                                break;

                            case InputDeviceType.Keyboard:
                                buttonState |= PollKeyboardButtonState(ref controlInputs);
                                break;

                            case InputDeviceType.Gamepad:
                                buttonState |= PollGamepadButtonState(ref controlInputs);
                                break;
                        }
                    }

                    state.value = (buttonState ? 1f : 0f);
                }else
                if(map.mappingType == InputMappingType.Axis)
                {
                    float axisValue = 0f;

                    foreach(Control control in map.controls)
                    {
                        Input[] controlInputs = control.inputs;

                        switch (control.deviceType)
                        {
                            case InputDeviceType.Mouse:
                                axisValue = PollMouseAxisState(ref controlInputs) * control.axisScale;
                                if (axisValue != 0)
                                    break;
                                continue;

                            case InputDeviceType.Keyboard:
                                axisValue = PollKeyboardAxisState(ref controlInputs) * control.axisScale;
                                if (axisValue != 0)
                                    break;
                                continue;

                            case InputDeviceType.Gamepad:
                                axisValue = PollGamepadAxisState(ref controlInputs) * control.axisScale;
                                if (axisValue != 0)
                                    break;
                                continue;
                        }

                        //
                        //  Explanation on this break/continue mess:
                        //    Axis type inputs use a 'first come, first server' approach,
                        //    meaning the first Control which returns a non-zero value will be used.
                        //  
                        //    We use continue on a Control which returned 0 for the axis to restart the loop,
                        //    or break if a non-zero value was returned for the axis, which in turn allows the break
                        //    below this comment to run, thus ending the loop.
                        //
                        //    There are potential problems however, mostly because of gamepad "stick drift".
                        //    
                        //  Why this solution?
                        //    - 'Performance'
                        //    - Normalization sucked. Having a axis value of 1 from the keyboard, but a value of 0.01
                        //      from a gamepad resulted in a final axis value of 0.505.
                        //
                        //  Why this comment?
                        //    I'm really fucking confused...
                        // 
                        break;
                    }

                    state.value = axisValue;
                }

                inputStates[key] = state;
            }
        }

        private static bool PollMouseButtonState(ref Input[] mappingInputs)
        {
            return glfwWindow.MouseState.IsButtonDown((MouseButton)mappingInputs[0]);
        }
        private static bool PollKeyboardButtonState(ref Input[] mappingInputs)
        {
            return glfwWindow.KeyboardState.IsKeyDown((Keys)mappingInputs[0]);
        }
        private static bool PollGamepadButtonState(ref Input[] mappingInputs)
        {
            foreach(JoystickState jsState in glfwWindow.JoystickStates)
            {
                if(jsState != null)
                {
                    if((int)mappingInputs[0] > jsState.ButtonCount)
                    {
                        continue;
                    }

                    return jsState.IsButtonDown((int)mappingInputs[0]);
                }
            }

            return false;
        }
        private static float PollMouseAxisState(ref Input[] mappingInputs)
        {
            MouseState ms = glfwWindow.MouseState;

            switch (mappingInputs[0])
            {
                case Input.MouseAxisX: return (ms.X - ms.PreviousX) * 0.1f;
                case Input.MouseAxisY: return (ms.Y - ms.PreviousY) * 0.1f;
                case Input.MouseAxisZ: return ms.Scroll.Y - ms.PreviousScroll.Y;
                
                default: break;
            }

            return 0f;
        }
        private static float PollKeyboardAxisState(ref Input[] mappingInputs)
        {
            float axisA = glfwWindow.KeyboardState.IsKeyDown((Keys)mappingInputs[0]) ? 1f : 0f;
            float axisB = glfwWindow.KeyboardState.IsKeyDown((Keys)mappingInputs[1]) ? 1f : 0f;

            return axisB - axisA;
        }
        private static float PollGamepadAxisState(ref Input[] mappingInputs)
        {
            foreach (JoystickState jsState in glfwWindow.JoystickStates)
            {
                if (jsState != null)
                {
                    if ((int)mappingInputs[0] > jsState.AxisCount)
                    {
                        continue;
                    }

                    //Need to chop some decimals or we're in deep shit.
                    return MathF.Round(jsState.GetAxis((int)mappingInputs[0]), 4);
                }
            }

            return 0f;
        }

        public static bool InputPressed(string name)
        {
            InputState state = inputStates[name];
            return (state.value != 0f) & (state.lastValue == 0f);
        }

        public static bool InputReleased(string name)
        {
            InputState state = inputStates[name];
            return (state.value == 0f) & (state.lastValue != 0f);
        }

        public static bool InputHeld(string name)
        {
            InputState state = inputStates[name];
            return (state.value != 0f) & (state.lastValue != 0f);

        }

        public static float InputValue(string name)
        {
            return inputStates[name].value;
        }

        private static void OnJoystickConnectDisconnect(JoystickEventArgs ev)
        {
            if(ev.IsConnected)
            {
                //Joystick was connected
                JoystickState state = glfwWindow.JoystickStates[ev.JoystickId];

                Log.Info($"Gamepad connected. Device Info = [name: {state.Name}, id: {state.Id}]");

                return;
            }

            //Joystick was disconnected
            Log.Info($"Gamepad disconnected. Device Info = [id: {ev.JoystickId}]");
            return;
        }
    }
}
