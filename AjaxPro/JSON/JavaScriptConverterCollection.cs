/*
 * MS	06-05-30	removed this code
 * 
 * 
 * 
 */
//using System;
//using System.Collections;

//namespace AjaxPro
//{
//    /// <summary>
//    /// Represents a collection of JavaScriptConverters.
//    /// </summary>
//    internal class JavaScriptConverterCollection : CollectionBase
//    {
//        #region CollectionBase Members

//        internal IJavaScriptConverter this[int index]
//        {
//            get  
//            {
//                return((IJavaScriptConverter)List[index]);
//            }
//            set  
//            {
//                List[index] = value;
//            }
//        }

//        internal int Add(IJavaScriptConverter value)  
//        {
//            return(List.Add(value));
//        }

//        internal int IndexOf(IJavaScriptConverter value)  
//        {
//            return(List.IndexOf(value));
//        }

//        internal void Insert(int index, IJavaScriptConverter value)  
//        {
//            List.Insert(index, value);
//        }

//        internal void Remove(IJavaScriptConverter value)
//        {
//            List.Remove(value);
//        }

//        internal bool Contains(IJavaScriptConverter value)  
//        {
//            // If value is not of type Int16, this will return false.
//            return(List.Contains(value));
//        }

//        internal IJavaScriptConverter GetConverter(Type t, JavaScriptConverterDirectionType serializationType)
//        {
//            Type[] types = null;

//            for(int i=0; i<List.Count; i++)			
//            {
//                switch(serializationType)
//                {
//                    case JavaScriptConverterDirectionType.Serialize:

//                        types = this[i].SerializableTypes;
//                        break;

//                    case JavaScriptConverterDirectionType.Deserialize:

//                        types = this[i].DeserializableTypes;
//                        break;
//                }

//                if(types == null)
//                    continue;

//                foreach(Type type in types)
//                {
//                    if(type.IsAssignableFrom(t))
//                        return this[i];
//                }
//            }

//            return null;
//        }

//        protected override void OnInsert(int index, Object value)
//        {
//            if(value.GetType().IsAssignableFrom(typeof(IJavaScriptConverter)))
//                throw new ArgumentException( "value must be of type IJavaScriptConverter.", "value" );
//        }

//        protected override void OnRemove( int index, Object value )  
//        {
//            if(value.GetType().IsAssignableFrom(typeof(IJavaScriptConverter)))
//                throw new ArgumentException( "value must be of type IJavaScriptConverter.", "value" );
//        }

//        protected override void OnSet( int index, Object oldValue, Object newValue )  
//        {
//            if(newValue.GetType().IsAssignableFrom(typeof(IJavaScriptConverter)))
//                throw new ArgumentException( "value must be of type IJavaScriptConverter.", "newValue" );
//        }

//        protected override void OnValidate(Object value)
//        {
//            if(value.GetType().IsAssignableFrom(typeof(IJavaScriptConverter)))
//                throw new ArgumentException( "value must be of type IJavaScriptConverter.", "value" );
//        }

//        #endregion
//    }
//}