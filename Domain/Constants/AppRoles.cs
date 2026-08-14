namespace Domain.Constants;

public static class AppRoles
{
    public const string SuperAdmin = nameof(SuperAdmin);
    public const string Admin = nameof(Admin);

    public const string CatalogManager = nameof(CatalogManager);
    public const string InventoryManager = nameof(InventoryManager);
    public const string ProcurementManager = nameof(ProcurementManager);
    public const string SalesManager = nameof(SalesManager);
    public const string SupportAgent = nameof(SupportAgent);

    public const string Customer = nameof(Customer);

    public const string Administrators =
        SuperAdmin + "," + Admin;

    public const string CatalogAdministrators =
        SuperAdmin + "," + Admin + "," + CatalogManager;

    public const string InventoryAdministrators =
        SuperAdmin + "," + Admin + "," + InventoryManager;

    public const string ProcurementAdministrators =
        SuperAdmin + "," + Admin + "," + ProcurementManager;

    public const string SalesAdministrators =
        SuperAdmin + "," + Admin + "," + SalesManager;

    public const string SupportUsers =
        SuperAdmin + "," + Admin + "," + SupportAgent;

    public static readonly string[] All =
    [
        SuperAdmin,
        Admin,
        CatalogManager,
        InventoryManager,
        ProcurementManager,
        SalesManager,
        SupportAgent,
        Customer
    ];
}