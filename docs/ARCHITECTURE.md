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

Each feature is implemented using the CQRS pattern, with separate command and query handlers for each operation. For that we're using the open source library `LiteBus` to implement the mediator pattern and decouple the layers.

# Vertical Slices
Each feature is implemented as a vertical slice, meaning that all the layers for a specific feature are grouped together.

Each Command/Query contains the handler within it (one Command class per feature), and the handler is responsible for executing the business logic for that feature.
I don't want Command/Query class and Handler class to be separate, as it would add unnecessary complexity and boilerplate code.

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

Each module is injected in the main API (`BuyOrRent.API`) using dependency injection, allowing for loose coupling and separation of concerns.

# Infrastructure
- SQL Server is used as the database, with Entity Framework Core as the ORM for data access.