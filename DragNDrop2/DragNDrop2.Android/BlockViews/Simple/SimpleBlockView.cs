using Android.Content;
using Android.Util;

namespace DragNDrop2.Droid.BlockViews.Simple
{
    public abstract class SimpleBlockView : BlockView
    {
        protected SimpleBlockView(Context context) : base(context)
        {
        }

        protected SimpleBlockView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public override bool IsContainer()
        {
            return false;
        }
    }
}