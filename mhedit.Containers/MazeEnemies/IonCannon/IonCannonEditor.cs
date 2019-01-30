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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

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

            /// Make a complete copy of the passed program to edit..
            editor.Program = DeepClone( (IonCannonProgram)value );

            DialogResult result = editor.ShowDialog();

            /// On user OK we can return the newly edited program, otherwise just return the original.
            return result == DialogResult.OK ? editor.Program : value;
        }

        public static T DeepClone<T>( T obj )
        {
            using ( var ms = new MemoryStream() )
            {
                var formatter = new BinaryFormatter();

                formatter.Serialize( ms, obj );

                ms.Position = 0;

                return (T)formatter.Deserialize( ms );
            }
        }
    }

}
