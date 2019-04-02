using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace mhedit.Containers.TypeConverters
{
    /// <summary>
    /// The VelocityTypeConverter allows the ProperyGrid to properly display Velocity properties in the 
    /// PropertyGrid window.
    /// </summary>
    public class SignedVelocityTypeConverter : TypeConverter
    {
        /// <summary>
        /// Returns a collection of properties for the type of array specified by the value parameter. 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="value"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context,
            object value,
            Attribute[] filter)
        {
            return TypeDescriptor.GetProperties(value, filter);
        }
        /// <summary>
        /// Returns whether this object supports properties. 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        // Overrides the CanConvertFrom method of TypeConverter.
        // The ITypeDescriptorContext interface provides the context for the
        // conversion. Typically, this interface is used at design time to 
        // provide information about the design-time container.
        /// <summary>
        /// Returns whether this converter can convert an object of one type to the type of this converter. 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context,
           Type sourceType)
        {

            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }
        // Overrides the ConvertFrom method of TypeConverter.
        /// <summary>
        /// Converts the given value to the type of this converter. 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object ConvertFrom(ITypeDescriptorContext context,
           CultureInfo culture, object value)
        {
            if (value is string)
            {
                throw new NotSupportedException( typeof( SignedVelocity ).Name );
            }
            return base.ConvertFrom(context, culture, value);
        }
        // Overrides the ConvertTo method of TypeConverter.
        /// <summary>
        /// Converts the given value object to the specified type. 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        public override object ConvertTo(ITypeDescriptorContext context,
           CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return ((SignedVelocity)value).X + "," + ((SignedVelocity)value).Y;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
