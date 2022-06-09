using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using mhedit.Containers.MazeEnemies.IonCannon;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

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

            using ( CannonProgramEditor editor = new CannonProgramEditor(MakeCopy( original ) ) )
            {
                DialogResult result = editor.ShowDialog();

                /// On user OK when there are edits, return the modified program,
                /// otherwise just return the original.
                return result ==
                       DialogResult.OK &&
                       editor.State == CannonProgramEditor.EditState.ProgramEditsOccured ?
                           editor.Program :
                           original;
            }
        }

        private static IonCannonProgram MakeCopy( IonCannonProgram source )
        {
            IonCannonProgram copy = DeepClone( source );

            for ( int index = 0; index < source.Count; index++ )
            {
                if ( !source[index].IsChanged )
                {
                    copy[index].AcceptChanges();
                }
            }

            return copy;
        }

        private static T DeepClone<T>( T obj )
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                XmlSerializer serializer = new XmlSerializer(obj.GetType());

                using (XmlWriter xmlWriter = XmlWriter.Create(memoryStream,
                    new XmlWriterSettings { Indent = true }))
                {
                    serializer.Serialize( xmlWriter, obj );
                }

                memoryStream.Position = 0;

                using (XmlReader xmlReader = XmlReader.Create(memoryStream))
                {
                    return (T)serializer.Deserialize(xmlReader);
                }
            }
        }
    }
}
