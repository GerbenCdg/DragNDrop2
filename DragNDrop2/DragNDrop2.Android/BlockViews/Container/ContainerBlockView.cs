using Android.Content;
using Android.Views;
using Android.Widget;

namespace DragNDrop2.Droid.BlockViews.Container
{
    public abstract class ContainerBlockView : BlockView
    {
        protected LinearLayout LL { get; private set; }
        private View Header { get; set; }
        private Container Container { get; }

        private ContainerBlockView(Context context, Container container) : base(context)
        {
            LL = new LinearLayout(context)
            {
                Orientation = Orientation.Vertical,
                LayoutParameters =
                    new FrameLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent)
            };
            AddView(LL);

            ViewGroup.LayoutParams param = LayoutParameters;
            param.Width = ViewGroup.LayoutParams.WrapContent;
            param.Height = ViewGroup.LayoutParams.WrapContent;
            LayoutParameters = param;

            Container = container;
            Container.SetOnDragListener((View.IOnDragListener)context);
            LL.AddView(container);
        }

        // TODO allow to use ConditionContainer as header
        protected ContainerBlockView(Context context, int layoutRes, Container container) : this(context, container)
        {
            SetHeader(LayoutInflater.From(context).Inflate(layoutRes, null));
        }

        protected ContainerBlockView(Context context, View header, Container container) : base(context)
        {
            SetHeader(header);
        }
        
        private void SetHeader(View header)
        {
            Header = header;
            LL.AddView(header, 0);
        }
        
        public override bool IsContainer()
        {
            return true;
        }

        public override Categories[] GetCategories()
        {
            return new[] {Categories.Containers};
        }

    }
}