# Code Review Checklist

Please check the following items before approving a Pull Request.

## General
- [ ] **Functionality**: Does the code do what it's supposed to do? Does it handle edge cases?
- [ ] **Readability**: Is the code easy to understand? Are variable and method names descriptive?
- [ ] **Simplicity**: Is there a simpler way to achieve the same result? Avoid over-engineering.

## Performance
- [ ] **Database Queries**: Are there any N+1 query problems? Are indexes being used effectively?
- [ ] **Loops**: Are there any heavy computations inside loops that can be optimized?
- [ ] **Async/Await**: Is asynchronous programming used correctly? Avoid `.Result` or `.Wait()`.

## Security
- [ ] **Input Validation**: Is all user input validated?
- [ ] **Authorization**: Are proper access checks in place?
- [ ] **Secrets**: Are any secrets (API keys, passwords) committed to the code? (They should NOT be).

## Testing
- [ ] **Unit Tests**: Are there sufficient unit tests for the changes?
- [ ] **Integration Tests**: Do the changes require integration tests?

## Style & Standards
- [ ] **Naming Conventions**: Do names follow the .NET naming conventions (PascalCase, camelCase)?
- [ ] **Formatting**: Is the code formatted correctly (indentation, spacing)? (Check `.editorconfig`)
- [ ] **Comments**: Are complex logic blocks explained with comments?
