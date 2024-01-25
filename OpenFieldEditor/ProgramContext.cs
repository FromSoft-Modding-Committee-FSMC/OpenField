using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenFieldEditor
{
    internal class ProgramContext : ApplicationContext
    {
        //Properties
        public Form? MainWindow
        {
            get => mainForm;
        }

        //Private Data
        private Form? mainForm;

        public ProgramContext(Form firstForm)
        {
            //Initialize the first primary form
            ChangePrimaryWindow(firstForm);
        }

        public void ChangePrimaryWindow(Form newForm)
        {
            Form? oldForm = mainForm;

            //Destroy the old form (if it's not null)
            if(oldForm != null)
            {
                oldForm.Close();
                oldForm.Dispose();
            }

            //Initialize our new main form
            mainForm = newForm;
            mainForm.Show();
        }

    }
}
