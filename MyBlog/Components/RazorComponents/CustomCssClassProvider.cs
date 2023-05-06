using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Components.RazorComponents;

public class CustomCssClassProvider<ProviderType> : ComponentBase
    where ProviderType : FieldCssClassProvider, new()
{
    [CascadingParameter]
    EditContext? CurrentEditContext { get; set; }
    public ProviderType Provider { get; set; } = new ProviderType();
    protected override void OnInitialized()
    {
        if (CurrentEditContext == null)
        {
            throw new InvalidOperationException(
                $"{nameof(CustomCssClassProvider<ProviderType>)} requires a cascading parameter of type {nameof(EditContext)}, for example, you can use {nameof(CustomCssClassProvider<ProviderType>)} inside and EditForm");

        }
        CurrentEditContext.SetFieldCssClassProvider(Provider);
    }
}