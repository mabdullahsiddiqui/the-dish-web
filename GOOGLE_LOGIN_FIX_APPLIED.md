# Google Login Fix Applied

## ✅ Changes Made

### 1. Fixed LINQ Translation Error

**Problem:** Entity Framework Core couldn't translate `ExternalProvider.ToString()` to SQL.

**Solution:** Changed the repository method to compare the enum directly instead of using `ToString()`.

**Files Modified:**

1. **`IUserRepository.cs`**:
   - Changed method signature from `string provider` to `ExternalProvider provider`
   - Added `using TheDish.User.Domain.Enums;`

2. **`UserRepository.cs`**:
   - Updated `GetByExternalProviderAsync` to accept `ExternalProvider` enum
   - Changed comparison from `u.ExternalProvider.ToString() == provider` to `u.ExternalProvider == provider`
   - Added `using TheDish.User.Domain.Enums;`

3. **`SocialLoginCommandHandler.cs`**:
   - Changed call from `GetByExternalProviderAsync(userInfo.Id, provider.ToString(), ...)` to `GetByExternalProviderAsync(userInfo.Id, provider, ...)`

## ✅ Answers to Your Questions

### 1. Do you need secret key?
**No** - For Google ID token validation, only the Client ID is needed. The Client Secret is only required for server-side OAuth flows (authorization code exchange).

### 2. Origin: `http://localhost:3000` or `http://localhost:3000/register`?
**Use `http://localhost:3000`** (without the path). The origin is just protocol + domain + port. Google will accept tokens from any path on that origin.

### 3. Error in User Service logs
**Fixed!** The LINQ translation error has been resolved by comparing the enum directly.

## Next Steps

1. **Backend services are already restarted** (from the restart script)
2. **Restart web app** to clear any cached errors:
   ```powershell
   # Stop current npm run dev (Ctrl+C)
   cd web
   npm run dev
   ```

3. **Test Google login**:
   - Go to http://localhost:3000/register
   - Click "Sign in with Google"
   - Should work now! ✅

## What Was Fixed

The error was:
```
The LINQ expression 'DbSet<User>().Where(u => u.ExternalProviderId == __providerId_0 && u.ExternalProvider.ToString() == __provider_1)' could not be translated.
Translation of method 'object.ToString' failed.
```

Now the query compares the enum directly:
```csharp
u.ExternalProvider == provider  // ✅ EF Core can translate this
```

Since `ExternalProvider` is stored as a string in the database (via `.HasConversion<string>()`), EF Core can compare the enum value directly with the database column.

## Expected Behavior

1. User clicks "Sign in with Google"
2. Google popup opens (if origin is configured correctly)
3. User selects account
4. Frontend sends ID token to backend
5. Backend validates token with Google ✅
6. Backend queries database for existing user ✅ (no more LINQ error)
7. Backend creates/links user account ✅
8. Backend returns JWT token ✅
9. Frontend stores token and redirects ✅
10. User is logged in! ✅

