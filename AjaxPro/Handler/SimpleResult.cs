/*
 * MS	06-06-07	changed to internal
 * 
 * 
 */
//using System;

//namespace AjaxPro
//{
//    internal class SimpleResult : IAsyncResult
//    {
//        private bool m_Completed = false;

//        public bool IsCompleted
//        {
//            get
//            {
//                return m_Completed;
//            }
//            set
//            {
//                lock(this)
//                {
//                    m_Completed = value;
//                }
//            }
//        }

//        #region Implementation of IAsyncResult

//        public object AsyncState
//        {
//            get
//            {
//                return null;
//            }
//        }

//        public bool CompletedSynchronously
//        {
//            get
//            {
//                return false;
//            }
//        }

//        public System.Threading.WaitHandle AsyncWaitHandle
//        {
//            get
//            {
//                return this.AsyncWaitHandle;
//            }
//        }

//        #endregion
//    }
//}
