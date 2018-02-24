using System.Runtime.InteropServices.WindowsRuntime;
using Android.Content;

namespace DragNDrop2.Droid.BlockViews.Container
{
    public class ForBlockView : ContainerBlockView
    {
        public ForBlockView(Context context) : base(context, Resource.Layout.CardviewFor, new InstructionContainer(context))
        {
        }

        public override BlockView Clone()
        {
            return new ForBlockView(Context);
        }

        public override string GetBlockName()
        {
            return "For loop";
        }
    }
}