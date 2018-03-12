using System;
using System.Collections.Generic;
using System.Text;

namespace EventManager.Abstractions
{
    public interface IEventDragAndDrop<T>
    {
        IEnumerable<T> Drag(DateTime fromIncluded, DateTime toExcluded);

        bool Drop(IEnumerable<T> events);
    }
}
