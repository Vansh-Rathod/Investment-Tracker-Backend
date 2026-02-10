# Repository & Controller Refactoring Implementation Plan

## Overview
Refactor all repositories and controllers to follow a consistent pattern:
- **ViewModels** for GET operations (from stored procedures)
- **Request DTOs** for CREATE/UPDATE operations
- **Entity models** for database operations
- **Separate controller endpoints** for each operation
- **APIResponse** wrapper for all responses

## Entities to Implement

### 1. AMC (Asset Management Company)
- [x] ViewModel: AMCViewModel
- [x] Request DTOs: CreateAMCRequest, UpdateAMCRequest
- [x] Entity: AMC
- [x] Repository Interface: IAMCRepository
- [x] Repository: AMCRepository
- [ ] Controller: AMCController (needs separate endpoints)

### 2. MutualFund
- [x] ViewModel: MutualFundViewModel
- [x] Request DTOs: CreateMutualFundRequest, UpdateMutualFundRequest
- [x] Entity: MutualFund
- [x] Repository Interface: IMutualFundRepository
- [x] Repository: MutualFundRepository
- [ ] Controller: MutualFundController (needs refactoring)

### 3. Stock
- [x] ViewModel: StockViewModel
- [x] Request DTOs: CreateStockRequest, UpdateStockRequest
- [x] Entity: Stock
- [x] Repository Interface: IStockRepository
- [x] Repository: StockRepository
- [ ] Controller: StockController (needs refactoring)

### 4. User
- [x] ViewModel: UserViewModel
- [x] Request DTOs: CreateUserRequest, UpdateUserRequest
- [x] Entity: User
- [x] Repository Interface: IUserRepository
- [x] Repository: UserRepository
- [ ] Controller: UserController (needs separate endpoints)

### 5. Portfolio
- [x] ViewModel: PortfolioViewModel
- [x] Request DTOs: CreatePortfolioRequest, UpdatePortfolioRequest
- [x] Entity: Portfolio
- [x] Repository Interface: IPortfolioRepository
- [x] Repository: PortfolioRepository (needs helper methods)
- [ ] Controller: PortfolioController (needs refactoring)

### 6. SIP
- [x] ViewModel: SIPViewModel
- [x] Request DTOs: CreateSIPRequest, UpdateSIPRequest
- [x] Entity: SIP
- [x] Repository Interface: ISIPRepository
- [x] Repository: SIPRepository (needs helper methods)
- [ ] Controller: SIPController (needs refactoring)

### 7. Transaction
- [x] ViewModel: TransactionViewModel
- [ ] Request DTOs: CreateTransactionRequest
- [x] Entity: Transaction
- [x] Repository Interface: ITransactionRepository
- [x] Repository: TransactionRepository
- [ ] Controller: TransactionController (needs refactoring)

### 8. Dashboard
- [x] ViewModels: DashboardSummaryViewModel, AllocationDataViewModel, PerformanceDataViewModel
- [x] Repository Interface: IDashboardRepository
- [x] Repository: DashboardRepository
- [ ] Controller: DashboardController (needs refactoring)

### 9. AssetType
- [x] ViewModel: AssetTypeViewModel
- [ ] Request DTOs: CreateAssetTypeRequest, UpdateAssetTypeRequest
- [x] Entity: AssetType
- [x] Repository Interface: IAssetTypeRepository
- [x] Repository: AssetTypeRepository
- [ ] Controller: AssetTypeController (needs separate endpoints)

### 10. Category
- [x] ViewModel: CategoryViewModel
- [ ] Request DTOs: CreateCategoryRequest, UpdateCategoryRequest
- [x] Entity: Category
- [x] Repository Interface: ICategoryRepository
- [x] Repository: CategoryRepository
- [ ] Controller: CategoryController (needs separate endpoints)

### 11. Exchange
- [x] ViewModel: ExchangeViewModel
- [ ] Request DTOs: CreateExchangeRequest, UpdateExchangeRequest
- [x] Entity: Exchange
- [x] Repository Interface: IExchangeRepository
- [x] Repository: ExchangeRepository
- [ ] Controller: ExchangeController (needs separate endpoints)

## Repository Pattern

### Interface Methods
```csharp
Task<DbResponse<List<EntityViewModel>>> GetAll(...filters);
Task<DbResponse<EntityViewModel>> GetById(int id);
Task<DbResponse<int>> Create(Entity entity);
Task<DbResponse<int>> Update(Entity entity);
Task<DbResponse<int>> Delete(int id);
```

### Helper Methods (in Repository)
```csharp
public async Task<EntityViewModel> GetByIdAsync(int id, int userId);
public async Task<int> CreateAsync(int userId, CreateEntityRequest request);
public async Task<bool> UpdateAsync(int userId, UpdateEntityRequest request);
public async Task<bool> DeleteAsync(int id, int userId);
```

## Controller Pattern

### Endpoints
```csharp
[HttpGet] GetAll(...filters) -> APIResponse<List<EntityViewModel>>
[HttpGet("{id}")] GetById(int id) -> APIResponse<EntityViewModel>
[HttpPost] Create([FromBody] CreateEntityRequest) -> APIResponse<int>
[HttpPut("{id}")] Update(int id, [FromBody] UpdateEntityRequest) -> APIResponse<bool>
[HttpDelete("{id}")] Delete(int id) -> APIResponse<bool>
```

## Implementation Steps

1. ✅ Create missing Request DTOs
2. ✅ Create missing ViewModels
3. ⏳ Update Repository Interfaces with helper methods
4. ⏳ Implement Repository helper methods
5. ⏳ Refactor Controllers to use separate endpoints
6. ⏳ Update all Controllers to return APIResponse
7. ⏳ Add proper logging to all operations
8. ⏳ Test compilation

## Notes
- All GET operations return ViewModels
- All CREATE/UPDATE operations accept Request DTOs
- Repository converts Request DTOs to Entity models
- Controllers always return APIResponse<T>
- All operations use ILoggingService
