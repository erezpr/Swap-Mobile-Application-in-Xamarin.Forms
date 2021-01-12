using Swap.Enums;
using Xamarin.Forms;

namespace Swap.Behaviors
{
    public class VideoItemLabelBehavior : Behavior<Label>
    {
        protected override void OnAttachedTo(Label bindable)
        {
            ItemType itemType = (Application.Current as App).ItemViewModel.ItemType;

            if (itemType == ItemType.VideoGame)
            {
                bindable.IsVisible = true;
            }
            else
            {
                bindable.IsVisible = false;
            }
        }
    }
}
