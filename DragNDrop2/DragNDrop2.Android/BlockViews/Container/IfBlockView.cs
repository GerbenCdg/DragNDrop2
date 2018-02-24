using Android.Content;

namespace DragNDrop2.Droid.BlockViews.Container
{
    public class IfBlockView : ContainerBlockView
    {
        public IfBlockView(Context context) : base(context, Resource.Layout.CardviewIf, new InstructionContainer(context))
        {
        }

        public override string GetBlockName()
        {
            return "If condition";
        }

        public override Categories[] GetCategories()
        {
            return new[] {Categories.Containers, Categories.Logic};
        }

        public override BlockView Clone()
        {
            return new IfBlockView(Context);
        }
    }
}