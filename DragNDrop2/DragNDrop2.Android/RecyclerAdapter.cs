using System.Collections.Generic;
using Android.Graphics.Drawables;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using DragNDrop2.Droid.BlockViews;
using DragNDrop2.Droid.BlockViews.Simple;
using Java.Lang;

namespace DragNDrop2.Droid
{
    public class RecyclerAdapter : RecyclerView.Adapter
    {
        public class ViewHolder : RecyclerView.ViewHolder
        {
            // displayed in the recyclerview
            public RecyclerBlockView RecyclerBv { get; private set; }
            public TextView TvBlockName { get; private set; }
            public ImageView Iv { get; private set; }

            public ViewHolder(RecyclerBlockView itemView) : base(itemView)
            {
                RecyclerBv = itemView;
                TvBlockName = RecyclerBv.FindViewById<TextView>(Resource.Id.tv_blockname);
                Iv = RecyclerBv.FindViewById<ImageView>(Resource.Id.rbv_imageview);
            }

            public void SetDrawable(Drawable drawable)
            {
                Iv.SetImageDrawable(drawable);

                if (drawable == null)
                {
                    Iv.Visibility = ViewStates.Gone;
                    TvBlockName.SetTextSize(ComplexUnitType.Sp, 16);
                }
                else
                {
                    Iv.Visibility = ViewStates.Visible;
                    TvBlockName.SetTextSize(ComplexUnitType.Sp, 12);
                }
            }
        }

        private MainActivity Ma { get; }
        private List<BlockView> BlockViews { get; set; }
        private List<BlockView> FilteredBvs { get; set; }
        private BlockView.Categories LastCategory { get; set; }

        public RecyclerAdapter(MainActivity ac, BlockView.Categories selectedCat)
        {
            Ma = ac;
            BlockViews = new List<BlockView>
            {
                new TextBlockView(ac, "Action 1"),
                new TextBlockView(ac, "Action 2"),
                new InstructionBlockView(ac, Resource.Drawable.turn_left, "Turn Left"),
                new InstructionBlockView(ac, Resource.Drawable.turn_right, "Turn Right")
                // new ForBlockView(ac),
                // new IfBlockView(ac)
            };

            FilteredBvs = new List<BlockView>();
            FilteredBvs.AddRange(BlockViews);
            LastCategory = selectedCat;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            RecyclerBlockView blockView = (RecyclerBlockView) LayoutInflater.From(parent.Context)
                .Inflate(Resource.Layout.RecyclerBlockView, parent, false);

            return new ViewHolder(blockView);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder h, int position)
        {
            ViewHolder holder = (RecyclerAdapter.ViewHolder) h;

            BlockView bv = FilteredBvs[position];
            holder.RecyclerBv.SetRealBv(bv);
            holder.TvBlockName.SetText(bv.GetBlockName(), TextView.BufferType.Normal);

            if (bv is InstructionBlockView ibv)
            {
                holder.SetDrawable(ibv.Drawable);
            }
            else
            {
                holder.SetDrawable(null);
            }
            holder.RecyclerBv.SetOnTouchListener(Ma);
            holder.RecyclerBv.SetOnDragListener(Ma);
        }

        public override int ItemCount => FilteredBvs.Count;

        public void SetFilter(BlockView.Categories category)
        {
            FilteredBvs.Clear();

            if (LastCategory.Equals(category))
            {
                return;
            }

            LastCategory = category;

            if (category.Equals(BlockView.Categories.All))
            {
                FilteredBvs.AddRange(BlockViews);
                NotifyDataSetChanged();
                return;
            }

            foreach (var bv in BlockViews)
            {
                foreach (var cat in bv.GetCategories())
                {
                    if (cat.Equals(category))
                    {
                        FilteredBvs.Add(bv);
                        break;
                    }
                }
            }
        }

        // TODO RecyclerBlockView.axml --> element "..." is not declared
    }
}