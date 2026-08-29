# Contributing Guide

## Code of Conduct
Be respectful, inclusive, and professional in all interactions.

## How to Contribute

### 1. Fork & Clone
```bash
git clone https://github.com/yourusername/global-intelligence-monitor.git
git checkout -b feature/your-feature-name
```

### 2. Create Your Changes
- Follow C# naming conventions (PascalCase for classes, camelCase for variables)
- Add XML documentation comments
- Write unit tests for new features
- Update CHANGELOG.md

### 3. Code Style
- Use `dotnet format` to auto-format code
- Maximum line length: 120 characters
- Use async/await for I/O operations
- Follow SOLID principles

### 4. Commit & Push
```bash
git add .
git commit -m "feat: add new feature"
git push origin feature/your-feature-name
```

### 5. Create Pull Request
- Describe your changes clearly
- Link related issues
- Wait for review and approval

## Areas We Need Help

- [ ] Localization (add more languages)
- [ ] Performance optimization
- [ ] UI/UX improvements
- [ ] Bug fixes
- [ ] Documentation
- [ ] Test coverage

## Testing

Run unit tests:
```bash
dotnet test
```

## Questions?

Open an issue or join our community forum at forum.globalintelligence.monitor
