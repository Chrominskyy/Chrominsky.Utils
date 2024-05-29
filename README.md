
# Chrominsky.Utils

`Chrominsky.Utils` is a collection of utility functions designed to simplify common programming tasks in C#. This library aims to provide reusable components that enhance productivity and code readability.

## Features

- **String Manipulation**: Functions for common string operations.
- **Date and Time Utilities**: Simplify date and time handling.
- **File I/O**: Utility functions for file reading and writing.
- **Math Operations**: Common mathematical functions and operations.

## Installation

You can install the package via NuGet:

```bash
dotnet add package Chrominsky.Utils
```

Or by adding it directly to your `.csproj` file:

```xml
<PackageReference Include="Chrominsky.Utils" Version="1.0.0" />
```

## Usage

Here's a basic example of how to use some of the utilities in this package:

```csharp
using Chrominsky.Utils;

class Program
{
    static void Main()
    {
        string reversedString = StringUtils.Reverse("Hello World");
        Console.WriteLine(reversedString); // Output: "dlroW olleH"

        DateTime today = DateUtils.GetToday();
        Console.WriteLine(today.ToString("yyyy-MM-dd"));
    }
}
```

## Contributing

Contributions are welcome! Please fork the repository and create a pull request with your changes.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Contact

For any questions or suggestions, feel free to open an issue or contact the repository owner.
