using System.ComponentModel;
using System.Text.Json.Serialization;

namespace OFC.Resource.Shader
{
	public struct SShaderSampler
	{
		public string name;
		public int tileU				= 0x812f;
		public int tileV				= 0x812f;
		public int filterMin			= 0x2703;
		public int filterMag			= 0x2600;
		public float filterBias			= -1f;
		public float filterAnisotropy	= 16f;

		//Fuck you .NET?..
		public SShaderSampler() { }
	}
}