using Swap.Enums;
using Swap.ViewModels;
using Xamarin.Forms;

namespace Swap.Behaviors
{
    public class BookItemLabelBehavior : Behavior<Label>
    {
        protected override void OnAttachedTo(Label bindable)
        {
            if (bindable == null)
                return;

            ItemViewModel itemViewModel = (Application.Current as App).ItemViewModel;
            ItemType itemType = itemViewModel.ItemType;

            if (itemType == ItemType.Book)
            {
                bindable.IsVisible = true;
            }
            else if (itemType == ItemType.VideoGame)
            {
                bindable.IsVisible = false;
            }
        }
    }
}
