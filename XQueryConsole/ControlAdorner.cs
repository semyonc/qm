using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows;

namespace XQueryConsole
{
    public class ControlAdorner : Adorner
    {
        private readonly FrameworkElement mAdorningElement;
        private AdornerLayer mLayer;

        public ControlAdorner(FrameworkElement adornedElement, FrameworkElement adorningElement)
            : base(adornedElement)
        {
            mAdorningElement = adorningElement;
            if (adorningElement != null)
                AddVisualChild(adorningElement);
        }

        protected override int VisualChildrenCount
        {
            get { return mAdorningElement != null ? 1 : 0; }
        }

        protected override System.Windows.Media.Visual GetVisualChild(int index)
        {
            if (index == 0 && mAdorningElement != null)
                return mAdorningElement;

            return base.GetVisualChild(index);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (mAdorningElement != null)
            {
                mAdorningElement.Arrange(new Rect(new Point(0, 0), AdornedElement.RenderSize));
            }

            return finalSize;
        }

        public void SetLayer(AdornerLayer layer)
        {
            mLayer = layer;
            mLayer.Add(this);
        }

        public void RemoveLayer()
        {
            if (mLayer != null)
            {
                mLayer.Remove(this);
                RemoveVisualChild(mAdorningElement);
            }
        }
    }
}
