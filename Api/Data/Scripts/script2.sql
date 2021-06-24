START TRANSACTION;

CREATE TABLE "Customers" (
    "Id" uuid NOT NULL,
    "TIN" text NOT NULL,
    "RcNumber" text NOT NULL,
    "AccountNumber" text NOT NULL,
    "Email" text NOT NULL,
    "PhoneNumber" text NOT NULL,
    "CompanyName" text NOT NULL,
    "Address" text NOT NULL,
    "IsDeleted" boolean NOT NULL,
    "Created" timestamp without time zone NOT NULL,
    "Modified" timestamp without time zone NULL,
    "DeletedAt" timestamp without time zone NULL,
    CONSTRAINT "PK_Customers" PRIMARY KEY ("Id")
);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20210617143556_CustomerOnboard', '5.0.7');

COMMIT;

