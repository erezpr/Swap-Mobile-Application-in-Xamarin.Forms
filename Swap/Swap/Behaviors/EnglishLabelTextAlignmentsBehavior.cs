using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace Swap.Behaviors
{
    public class EnglishLabelTextAlignmentsBehavior : Behavior<Label>
    {
        protected override void OnAttachedTo(Label bindable)
        {
            Regex pattern = new Regex("^[a-zA-Z0-9. -_?]*$", RegexOptions.Compiled);

            if (bindable.Text != null && pattern.IsMatch(bindable.Text))
            {
                bindable.HorizontalTextAlignment = TextAlignment.End;
            }
        }
    }
}
