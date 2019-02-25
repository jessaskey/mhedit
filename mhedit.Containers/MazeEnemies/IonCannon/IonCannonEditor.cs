using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
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
            /// Make a complete copy of the passed program to edit..
            IonCannonProgram original = (IonCannonProgram)value;
            IonCannonProgram copy = DeepClone( original );

            /// Force copy to have the same "edits"
            IEnumerator<IonCannonInstruction> enumeratorOriginal = original.GetEnumerator();
            IEnumerator<IonCannonInstruction> enumeratorCopy = copy.GetEnumerator();

            /// Update the copy to have the same IsDirty signature as the original
            while ( enumeratorOriginal.MoveNext() && enumeratorCopy.MoveNext() )
            {
                enumeratorCopy.Current.IsDirty = enumeratorOriginal.Current.IsDirty;
            }

            CannonProgramEditor editor = new CannonProgramEditor( copy );

            DialogResult result = editor.ShowDialog();

            /// On user OK we can return the newly edited program, otherwise just return the original.
            return result == DialogResult.OK ? editor.Program : original;
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
