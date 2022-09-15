<!-- TODO: Discuss what scopes to use, if any at all, and what language should be used for documentation as well as comments. -->

# Contributing

## Summary

This project follows a set of standards and conventions to enforce unity amongs the code base and prevent conflicts.

## Standards

### GitHub Flow

The [GitHub Flow](https://docs.github.com/en/get-started/quickstart/github-flow) is a way of creating and managing branches and code in a git project. In essence it boils down to having a long-lived `main` branch where product ready code is held and several short-lived `feature` branches for modifying the software. Once the modification is complete, a pull request is made to merge the `feature` branch with the `main` branch.

This project has a naming convention for the `feature` branches that looks like this "`<type>-<issue reference>-<topic>`".

The `type` is a descriptor of the main purpose of the branch according to this list:

-   `feat` - features
-   `fix` - bug fixes
-   `style` - syntactice changes in the code, such as indents, comments, etc. that does not affect the code
-   `docs` - changes in the documentation, README, CONTRIBUTION, LICENSE, or general doc files.
-   `refactor` - refactoring production code, renaming a variable, moving code to another file, etc.
-   `test` - adding or changing tests of the code
-   `chore` - updating grunt tasks with no production code changes, such as changing config files, and does not fit any other type

The `type` is followed by a "`!`" for breaking changes, i.e. changes that make code not backwards compatible.

If it addresses an issue, the issue code is placed between the `type` and the `topic`.

The `topic` is a short and concise description of what the topic is of the branch, spaces are replaced by underscores.

Examples:

-   `docs-contributing_file`
-   `feat-live_client_api_wrapper`
-   `fix-41-undefined_screen`

### Conventional Commits standard

[Conventional Commits](https://www.conventionalcommits.org/en/v1.0.0-beta.2/) standard makes commit messages and bodies more clear and concise. Each commit should only contain small pieces or closely related changed. They follow this structure:

```
`<type>`[optional scope]: <description>

[optional body]

[optional footer]
```

The `type` is the purpose of the commit:

-   `feat` - features
-   `fix` - bug fixes
-   `style` - syntactice changes in the code, such as indents, comments, etc. that does not affect the code
-   `docs` - changes in the documentation, README, CONTRIBUTION, LICENSE, or general doc files.
-   `refactor` - refactoring production code, renaming a variable, moving code to another file, etc.
-   `test` - adding or changing tests of the code
-   `chore` - updating grunt tasks with no production code changes, such as changing config files, and does not fit any other type

The `type` is followed by a "`!`" for breaking changes, i.e. changes that make code not backwards compatible.

The `scope` of the type is to specify further what the commit changes, some examples are `database`, `api`, `ui`, `input`, etc. This is dependant on the project and should be discussed.

The `description` is a very short text that describes what the commit changes.

The `body` of the commit further describes the changes. If a breaking change is made or it addresses an issue it has a special syntax, see [examples](https://www.conventionalcommits.org/en/v1.0.0-beta.2/#examples).
