namespace InventoryManagementSystemCommon
{
    public class ResponseMessageConstants
    {
        public const string UserNotFoundErrorMessage = "User with ID '{0}' not found.";

        public const string FindUserByUsernameErrorMessage = "User with username '{0}' not found.";

        public const string FailedRoleChangeErrorMessage = "Failed to change role.";

        public const string FailedUserCreateErrorMessage = "Failed to create user.";

        public const string FailedRoleAssignmentErrorMessage = "Failed to add user to role.";

        public const string IncorrectPasswordErrorMessage = "Incorrect password.";

        public const string InvalidUserDataErrorMessage = "Invalid user data.";

        public const string ItemNotFoundErrorMessage = "Item not found.";

        public const string SuccessfullyCreatedUserMessage = "User created successfully.";

        public const string SuccessfullyChangedUserRoleMessage = "Role changed successfully for user '{0}'.";

        public const string SuccessfullyAddedBulkItemsMessage = "Bulk items added successfully.";

        public const string SuccessfullyDeletedItemMessage = "Item deleted successfully.";
    }
}
