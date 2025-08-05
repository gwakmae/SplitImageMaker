# Split Image Maker

A powerful WPF application for creating split-screen image compositions with customizable grid layouts and captions.

## 🌟 Features

- **Flexible Grid System**: Create custom grid layouts (2x2, 3x3, or any custom configuration)
- **Clipboard Integration**: Paste images directly from clipboard with Ctrl+V
- **Caption Support**: Add text captions to each panel with automatic styling
- **Real-time Preview**: See your combined image as you work
- **High-Quality Output**: Export final compositions in PNG or JPEG format
- **Intuitive Interface**: Click-to-select panels with visual feedback
- **Memory Efficient**: Optimized image handling to prevent memory leaks

## 🚀 Quick Start

1. **Clone the repository**
 ```bash
 git clone https://github.com/yourusername/SplitImageMaker.git
 cd SplitImageMaker
 ```

2. **Open in Visual Studio**
 - Open `SplitImageMaker.sln` in Visual Studio 2022
 - Build and run the project

3. **Start Creating**
 - Set your desired grid size (e.g., 2x2, 3x3)
 - Click on a panel to select it
 - Copy an image to clipboard and press Ctrl+V to paste
 - Add captions if needed
 - Save your combined image

## 📸 Screenshots

### Main Interface
The application features a clean, intuitive interface with:
- Grid configuration controls
- Visual panel selection
- Real-time preview pane
- Panel information sidebar

### Grid Layouts
Support for various grid configurations:
- 2x2 (4 panels)
- 3x3 (9 panels)
- Custom ratios and dimensions

## 🛠️ Technical Details

### Built With
- **.NET 8.0** - Modern .NET framework
- **WPF** - Windows Presentation Foundation for rich UI
- **MVVM Pattern** - Clean separation of concerns
- **Windows Forms Integration** - Enhanced clipboard support

### Key Components

#### Services
- **ImageService**: Handles image processing, cropping, and combining
- **GridService**: Manages grid layout and panel creation
- **FileService**: Handles file I/O operations

#### Models
- **GridConfiguration**: Stores grid layout settings and ratios
- **PanelInfo**: Represents individual panels with images and captions
- **SelectionArea**: Manages image selection areas

#### Helpers
- **BitmapHelper**: Comprehensive bitmap manipulation utilities
- **ClipboardHelper**: Robust clipboard image handling with memory safety

### Architecture Highlights

```
SplitImageMaker/
├── Models/           # Data models and configurations
├── Views/            # XAML UI and code-behind
├── ViewModels/       # MVVM view models
├── Services/         # Business logic services
└── Helpers/          # Utility classes
```

## 🎯 Use Cases

- **Social Media Content**: Create Instagram carousels and Facebook posts
- **Presentations**: Build comparison slides and before/after layouts
- **Documentation**: Generate step-by-step visual guides
- **Portfolio**: Showcase multiple works in organized layouts
- **Tutorials**: Create instructional image sequences

## 📋 System Requirements

- **OS**: Windows 10 or later
- **Framework**: .NET 8.0 Runtime
- **Memory**: 4GB RAM minimum
- **Storage**: 100MB available space

## 🔧 Configuration

### Grid Customization
- Modify rows and columns in the top toolbar
- Adjust panel ratios for asymmetric layouts
- Real-time grid updates without losing content

### Output Settings
- Default resolution: 1920x1080
- Supported formats: PNG, JPEG
- Quality settings for JPEG export

## 🤝 Contributing

We welcome contributions! Please follow these steps:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

### Development Guidelines
- Follow MVVM pattern
- Add XML documentation for public methods
- Include unit tests for new features
- Maintain backward compatibility

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🐛 Known Issues

- Large images (>4000px) may require additional processing time
- Clipboard monitoring requires Windows focus
- Some image formats may need conversion for optimal display

## 🔮 Roadmap

- [ ] Drag & drop image support
- [ ] Batch processing capabilities
- [ ] Template system for common layouts
- [ ] Export to multiple formats simultaneously
- [ ] Cloud storage integration
- [ ] Mobile companion app

## 📞 Support

- **Issues**: [GitHub Issues](https://github.com/yourusername/SplitImageMaker/issues)
- **Discussions**: [GitHub Discussions](https://github.com/yourusername/SplitImageMaker/discussions)
- **Email**: support@splitimagemaker.com

## 🙏 Acknowledgments

- Built with love using WPF and .NET 8.0
- Inspired by the need for simple, powerful image composition tools
- Thanks to the open-source community for continuous inspiration

---

⭐ **Star this repository if you find it useful!**

Made with ❤️ by [Your Name]