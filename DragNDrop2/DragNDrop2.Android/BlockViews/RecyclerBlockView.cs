using Android.Content;
using Android.Util;
using Android.Views;
using DragNDrop2.Droid.BlockViews.Simple;

namespace DragNDrop2.Droid.BlockViews
{
    public class RecyclerBlockView : SimpleBlockView
    {
        private BlockView CloneInstance { get; set; }
        private BlockView RealBv { get; set; }

        public RecyclerBlockView(Context context) : base(context)
        {
        }

        public RecyclerBlockView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public void SetRealBv(BlockView rbv)
        {
            if (RealBv != null)
            {
                RemoveView(RealBv);
            }

            RealBv = rbv;
            // RealBv.Visibility = ViewStates.Gone;
            // ((ViewGroup) RealBv.Parent)?.RemoveView(RealBv);
            // AddView(RealBv);
        }


        // If this RecyclerBlockView already contains a RealBv, remove it, and add a clone of the RealBv in this. 
        // We need to make a clone each time a DragNDrop starts from the RecyclerView to keep unique views in the Main LinearLayout
        private BlockView SetCloneInstance()
        {
            if (CloneInstance != null)
            {
                RemoveCloneInstance();
            }

            CloneInstance = RealBv.Clone();
            AddView(CloneInstance);
            return CloneInstance;
        }

        public void RemoveCloneInstance()
        {
            RemoveView(CloneInstance);
            CloneInstance = null;
        }

        public override BlockView Clone()
        {
            return SetCloneInstance();
        }

        public override Categories[] GetCategories()
        {
            return new[] {Categories.Others};
        }

        public override string GetBlockName()
        {
            return "RecyclerBlockView";
        }

        public override string ToString()
        {
            var str = "RecyclerBlockView : {\n "  + "\n}";
            return str;
        }

       
    }
}