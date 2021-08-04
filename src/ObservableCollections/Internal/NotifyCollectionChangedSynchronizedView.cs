﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace ObservableCollections.Internal
{
    internal class NotifyCollectionChangedSynchronizedView<T, TView> : INotifyCollectionChangedSynchronizedView<T, TView>
    {
        readonly ISynchronizedView<T, TView> parent;

        public NotifyCollectionChangedSynchronizedView(ISynchronizedView<T, TView> parent)
        {
            this.parent = parent;
            this.parent.RoutingCollectionChanged += Parent_RoutingCollectionChanged;
        }

        private void Parent_RoutingCollectionChanged(in NotifyCollectionChangedEventArgs<T> e)
        {
            CollectionChanged?.Invoke(this, e.ToStandardEventArgs());
        }

        public object SyncRoot => parent.SyncRoot;

        public int Count => parent.Count;

        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        public event Action<NotifyCollectionChangedAction>? CollectionStateChanged
        {
            add { parent.CollectionStateChanged += value; }
            remove { parent.CollectionStateChanged -= value; }
        }

        public event NotifyCollectionChangedEventHandler<T>? RoutingCollectionChanged
        {
            add { parent.RoutingCollectionChanged += value; }
            remove { parent.RoutingCollectionChanged -= value; }
        }

        public void AttachFilter(ISynchronizedViewFilter<T, TView> filter) => parent.AttachFilter(filter);
        public void ResetFilter(Action<T, TView>? resetAction) => parent.ResetFilter(resetAction);
        public INotifyCollectionChangedSynchronizedView<T, TView> WithINotifyCollectionChanged() => this;
        public void Dispose()
        {
            this.parent.RoutingCollectionChanged -= Parent_RoutingCollectionChanged;
            parent.Dispose();
        }

        public IEnumerator<(T, TView)> GetEnumerator() => parent.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => parent.GetEnumerator();
    }
}