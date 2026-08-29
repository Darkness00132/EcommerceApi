# ElectroShop — Product Requirements Document

## Vision & Purpose
ElectroShop is a scalable e-commerce backend for digital and consumer electronics retail. It combines catalog, inventory, checkout, payments, procurement, identity, and customer engagement capabilities using Clean Architecture and domain-driven design principles.

## Product Goals
- Provide a reliable catalog and purchasing experience.
- Prevent inventory depletion and overselling during concurrent checkouts.
- Preserve product and pricing history for completed orders.
- Support granular role-based access for business operations.
- Keep persistence, storage, payment, and notification integrations replaceable.

## Functional Requirements
### Identity & Access
- Support registration, email confirmation, authentication, refresh sessions, and sign-out.
- Support JWT bearer authentication for native clients.
- Store web refresh tokens in secure, HttpOnly cookies.
- Support two-factor authentication, lockout controls, roles, and policies.

### Catalog
- Organize products by categories and brands.
- Support bilingual catalog names and descriptions where applicable.
- Support SKUs, prices, active status, dynamic attributes, and product galleries.
- Support configurable discounts with values and validity periods.
- Snapshot product names, SKUs, and prices when orders are created.

### Cart & Checkout
- Provide one persistent cart per customer.
- Validate product availability and quantity limits.
- Calculate subtotal, discounts, shipping fees, and totals.
- Validate promo codes against minimum orders, usage limits, and validity periods.
- Reserve stock during concurrent checkout attempts.

### Orders
- Store delivery address, delivery notes, promo code, and financial totals.
- Keep order lines immutable after creation.
- Follow the state flow Pending → Processing → Shipped → Delivered or Cancelled.
- Allow customers to view their orders and authorized staff to process them.

### Payments
- Record payment status, amount, paid date, refund date, and payment attempts.
- Record payment methods, transaction identifiers, and gateway responses.
- Abstract payment gateways behind application contracts.
- Make payment callbacks and retries idempotent.

### Inventory & Procurement
- Track available stock, reserved stock, reorder levels, and inventory transactions.
- Reserve stock atomically and release reservations after failed or cancelled checkouts.
- Manage suppliers, purchase orders, purchase order items, goods receipts, and received quantities.
- Update inventory through auditable transactions when goods are received.

### Reviews, Newsletter & Media
- Allow eligible customers to review purchased products.
- Allow support agents to moderate reviews.
- Support newsletter subscription, unsubscribe, and duplicate prevention.
- Validate media MIME types and file sizes and return secure URI references.
- Support local storage in development and Azure Blob Storage in production.

## Domain Entities
- **Identity:** AppUser, AppRole, RefreshToken
- **Catalog:** Category, Brand, Product, ProductImage, Discount
- **Commerce:** Cart, CartItem, Order, OrderItem, PromoCode
- **Payments:** Payment, PaymentAttempt
- **Operations:** Inventory, InventoryTransaction, Supplier, PurchaseOrder, PurchaseOrderItem, GoodsReceipt, GoodsReceiptItem
- **Engagement:** Review, NewsletterSubscriber

## Roles
| Role | Responsibilities |
|---|---|
| SuperAdmin / Admin | Full administrative control |
| CatalogManager | Categories, brands, products, discounts, and images |
| InventoryManager | Stock, reservations, alerts, and transactions |
| ProcurementManager | Suppliers, purchase orders, and goods receipts |
| SalesManager | Orders, payments, refunds, and promo codes |
| SupportAgent | Customer orders, reviews, and support |
| Customer | Browsing, cart, checkout, payments, and reviews |

## Non-Functional Requirements
- The Domain layer must not reference infrastructure, presentation frameworks, databases, or third-party packages.
- Commands and queries must remain separated in the Application layer using MediatR.
- Domain rules must be unit-testable without databases or external services.
- Inventory and payment workflows must be transactional and idempotent where required.
- Nullable reference types, secure token handling, authorization policies, and observability are required.
- The solution must support local development and deployment to Azure SQL Database, Azure Blob Storage, and Azure-hosted application services.

## Success Criteria
- Valid purchases complete without inventory overselling.
- Completed orders retain accurate product and price snapshots.
- Failed or cancelled checkouts release reserved stock.
- Payment retries do not create duplicate payments or orders.
- Staff can perform only operations allowed by their roles.
- Domain tests run independently of infrastructure.
