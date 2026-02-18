The project follows a modular monolith architecture.

# Code Structure
Each module has its own directory under the `src` / `Modules` directory.
- `src/Modules/ModuleName`: Contains the code for a specific module.
The code in each module follows Clean Architecture principles, with separate layers for:
- Domain: Contains the core business logic and entities.
- Application: Contains the application services and use cases.
- Infrastructure: Contains the implementation details, such as database access and external services.
- API: Contains the controllers and API endpoints for the module.

Each feature within a module is following the Vertical Slice architecture, where all the layers for a specific feature are grouped together.

# Example
For example, if we have a `User` module, the minimal structure would look like this:
```
src/
  Modules/
	Users/ <-- Pluralized module name
	  Domain/
		Entities/
		  User.cs
	  Application/
		Features/
		  CreateUser/
		  CreateUserCommand.cs
		  DeleteUser/
		  DeleteUserCommand.cs
	  Infrastructure/
		Repositories/
		  UserRepository.cs
	  API/
		Controllers/
		  UserAPI.cs <-- Minimal API controller for user-related endpoints
```