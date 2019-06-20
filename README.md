# ImGui.Wpf

## About

ImGui.Wpf is a WPF implementation of an immediate mode graphical user interface. Other examples of ImGui implementations are things such as Unity3ds ImGui API and Dear ImGui.

The idea of an immediate mode Gui is to allow a simple API for creating quick and easy user interfaces. In my opinion ImGui implementations lack the advanced features that a full UI API provide (such as something like Xaml), but allow for quick prototyping and mock ups.

## Why

The main reason behind this library is that I needed a way to create graphical user interfaces within a scripting system. By which I mean runtime compiled C# being used as a scripting language. 

Using C# as a scripting language is made possible with the Roslyn compiler and or by third party libraries such as CS Script. However, runtime compiled Xaml or Forms becomes a little more complicated and also defeats the purpose (in my opinion) in exposing a simple to use scripting API. Thus ImGui.Wpf was born.

## State

**This is very much still a proof of concept and should not be used in a production environment.**

My current plan it to get all the controls implemented and an API fleshed out. Once that has been completed performance and scalability will be investigated and improved.

## Example

```csharp
var sliderValue = 0.5;
var buffer = "Hello World";

// Create an instance of an imGui panel.
// If no parameter is passed, the main application window is used as the
// hosting control, however you can pass any FrameworkElement which implements
// IAddChild as a hosting control.
using (var imGui = await ImGuiWpf.BeginPanel())
{
    while (true)
    {
        // All ImGui.Wpf calls must be wrapped in a Begin and End frame.
        await imGui.BeginFrame();

        // Add a TextBlock to the UI.
        await imGui.Text("Hello World {0}", 123);
        
        // Add a button to the UI, which will return true if the button was clicked.
        if (await imGui.Button("Save"))
        {
            MessageBox.Show($"You clicked save!\n{buffer}");
        }
        
        // Add a TextBox control, because we are async we can't use 'ref' so have to 
        // pass in the variable and return the potentially updated contents.
        buffer = await imGui.InputText("Input Text:", buffer);
        
        // Add a Slider control with a value, minimum and maximum.
        sliderValue = await imGui.Slider("Slider:", sliderValue, 0.0, 1.0);

        // End the frame.
        await imGui.EndFrame();
    }
}
```

## Implemented Controls

 * Button :white_check_mark:
 * CheckBox :x:
 * ComboBox :x:
 * Label :x:
 * ListBox :x:
 * ProgressBar :x:
 * RadioButton :x:
 * Slider :white_check_mark:
 * TextBlock :white_check_mark:
 * TextBox :white_check_mark:
 * ToggleButton :x:

## Additional Features

 * Custom Styling :x:
 * Layout Serialization :x:
 * Custom Controls :x:
