using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using mhedit.Containers.MazeEnemies;
using mhedit.Containers.MazeEnemies.IonCannon;

namespace mhedit.Containers
{
    // This class demonstrates the use of a custom UITypeEditor. 
    // It allows the MarqueeBorder control's LightShape property
    // to be changed at design time using a customized UI element
    // that is invoked by the Properties window. The UI is provided
    // by the LightShapeSelectionControl class.
    internal class IonCannonEditor : UITypeEditor
    {

        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            CannonProgramEditor editor = new CannonProgramEditor();


            editor.Program = (IonCannonProgram)value;

            /// BUG: Not making copies of the commands....
            IonCannonProgram savedProgram = editor.Program;

            DialogResult result = editor.ShowDialog();
            if (result == DialogResult.Cancel)
            {
                //restore previous movements
                return savedProgram;
            }
            return editor.Program;
        }
    }

}
