namespace NaturalFacade.App.UI
{
    public class Model
    {
        /// <summary>the list of listeners.</summary>
        private List<WeakReference<IModelListener>> m_listenerList = new List<WeakReference<IModelListener>>();

        /// <summary>Adds a listener to the model.</summary>
        public void AddListener(IModelListener listener)
        {
            m_listenerList.Add(new WeakReference<IModelListener>(listener));
        }

        /// <summary>Adds a listener to the model.</summary>
        public void RemoveListener(IModelListener listener)
        {
            List<WeakReference<IModelListener>> newListenerList = new List<WeakReference<IModelListener>>();
            foreach (WeakReference<IModelListener> weakRefListener in m_listenerList)
            {
                IModelListener strongRefListener = null;
                if (weakRefListener.TryGetTarget(out strongRefListener))
                {
                    if (object.ReferenceEquals(strongRefListener, listener) == false)
                    {
                        newListenerList.Add(weakRefListener);
                    }
                }
            }
            m_listenerList = newListenerList;
            m_listenerList.Add(new WeakReference<IModelListener>(listener));
        }

        /// <summary>Adds a listener to the model.</summary>
        public void OnDataChanged(string context=null)
        {
            foreach (WeakReference<IModelListener> weakRefListener in m_listenerList)
            {
                IModelListener strongRefListener = null;
                if (weakRefListener.TryGetTarget(out strongRefListener))
                {
                    strongRefListener.OnDataChanged(this, context);
                }
            }
        }
    }

    public interface IModelListener
    {
        /// <summary>Called when model data changes.</summary>
        void OnDataChanged(Model model, string context);
    }
}
