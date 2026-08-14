# Product Requirements Document (PRD) - Ecommerce API

## Overview
This Ecommerce API manages the back-end operations of a comprehensive e-commerce platform. It provides a robust, scalable architecture to handle users, catalogs, carts, sales orders, payments, promotions, reviews, inventory, procurement, and newsletters.

## Entities and Domains

### 1. Identity Area
- **APP_USER**: Manages user details (FirstName, LastName, Email, PhoneNumber, TwoFactorEnabled, Lockout details).
- **APP_ROLE**: Manages roles for users (Name, ConcurrencyStamp).
- **REFRESH_TOKEN**: Tracks refresh tokens for user authentication (Token, ExpiresAt, RevokedAt).

### 2. Catalog Area
- **CATEGORY**: Categorization of products with English and Arabic names/descriptions and an ImageKey.
- **BRAND**: Brands associated with products, supporting bilingual names.
- **PRODUCT**: The core entity for items sold. Includes SKU, price, activity status, and references Category, Brand, and Discount.
- **PRODUCT_IMAGE**: Manages multiple images per product.
- **DISCOUNT**: Represents discounts applied to products (DiscountType, Value, Validity dates).

### 3. Cart Area
- **CART**: Associated 1:1 with an AppUser.
- **CART_ITEM**: Items within a user's cart including quantity, unit price, and discount amount.

### 4. Sales Orders Area
- **ORDER**: Stores order details (Status, Subtotal, ShippingFee, Total, Address, DeliveryNotes, PromoCode).
- **ORDER_ITEM**: Contains items purchased within an order.

### 5. Payments Area
- **PAYMENT**: Records payments for an order (Status, Amount, PaidAt, RefundedAt).
- **PAYMENT_ATTEMPT**: Detailed record of payment attempts (Method, GatewayResponse, TransactionId).

### 6. Promotions Area
- **PROMO_CODE**: Promo codes for orders (Code, DiscountType, MinimumOrder, UsageLimit, UsedCount, Validity dates).

### 7. Reviews Area
- **REVIEW**: User reviews for products, including rating and comment.

### 8. Inventory Area
- **INVENTORY**: Tracks available stock and reorder levels per product.
- **INVENTORY_TRANSACTION**: Logs all changes to inventory (e.g., due to orders or goods receipts).

### 9. Procurement Area
- **SUPPLIER**: Vendor details (Name, Contact, TaxNumber).
- **PURCHASE_ORDER**: POs sent to suppliers.
- **PURCHASE_ORDER_ITEM**: Items within a PO.
- **GOODS_RECEIPT**: Records of received goods against a PO.
- **GOODS_RECEIPT_ITEM**: Detailed items received.

### 10. Newsletter Area
- **NEWSLETTER_SUBSCRIBER**: Subscriptions to the platform newsletter.

## Core Workflows and Roles

### Role-Based Access Control (RBAC)
The system uses granular roles rather than a simple Admin/Customer binary:
- **SuperAdmin & Admin**: Full administrative control across all domains.
- **CatalogManager**: Specializes in managing categories, brands, products, discounts, and product images.
- **InventoryManager**: Manages stock levels, reorder alerts, and inventory transactions.
- **ProcurementManager**: Handles suppliers, purchase orders, and goods receipts.
- **SalesManager**: Oversees sales orders, payments, and promo codes.
- **SupportAgent**: Assists with customer orders, reviews, and general support.
- **Customer**: Standard user who browses, adds to cart, and purchases items.

### User Journeys
1. **Customer Journey**: User registers, confirms email, browses the catalog, adds products to the cart, applies a promo code, places an order, and pays via a gateway.
2. **Management Journey**: The various managers (Catalog, Inventory, Procurement, Sales) collaborate to maintain the product lifecycle, keep stock updated via suppliers, and fulfill customer orders based on their specific access roles.
