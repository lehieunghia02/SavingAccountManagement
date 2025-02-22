create table AspNetRoles
(
    Id               nvarchar(450) not null
        constraint PK_AspNetRoles
            primary key,
    Name             nvarchar(256),
    NormalizedName   nvarchar(256),
    ConcurrencyStamp nvarchar(max)
)
    go

create table AspNetRoleClaims
(
    Id         int identity
        constraint PK_AspNetRoleClaims
            primary key,
    RoleId     nvarchar(450) not null
        constraint FK_AspNetRoleClaims_AspNetRoles_RoleId
            references AspNetRoles
            on delete cascade,
    ClaimType  nvarchar(max),
    ClaimValue nvarchar(max)
)
    go

create index IX_AspNetRoleClaims_RoleId
    on AspNetRoleClaims (RoleId)
    go

create unique index RoleNameIndex
    on AspNetRoles (NormalizedName)
    where [NormalizedName] IS NOT NULL
go

create table AspNetUsers
(
    Id                   nvarchar(450) not null
        constraint PK_AspNetUsers
            primary key,
    CCCD                 nvarchar(12)  not null,
    Address              nvarchar(50)  not null,
    FullName             nvarchar(50)  not null,
    SoDuTaiKhoan         float         not null,
    UserName             nvarchar(256),
    NormalizedUserName   nvarchar(256),
    Email                nvarchar(256),
    NormalizedEmail      nvarchar(256),
    EmailConfirmed       bit           not null,
    PasswordHash         nvarchar(max),
    SecurityStamp        nvarchar(max),
    ConcurrencyStamp     nvarchar(max),
    PhoneNumber          nvarchar(max),
    PhoneNumberConfirmed bit           not null,
    TwoFactorEnabled     bit           not null,
    LockoutEnd           datetimeoffset,
    LockoutEnabled       bit           not null,
    AccessFailedCount    int           not null
)
    go

create table AspNetUserClaims
(
    Id         int identity
        constraint PK_AspNetUserClaims
            primary key,
    UserId     nvarchar(450) not null
        constraint FK_AspNetUserClaims_AspNetUsers_UserId
            references AspNetUsers
            on delete cascade,
    ClaimType  nvarchar(max),
    ClaimValue nvarchar(max)
)
    go

create index IX_AspNetUserClaims_UserId
    on AspNetUserClaims (UserId)
    go

create table AspNetUserLogins
(
    LoginProvider       nvarchar(450) not null,
    ProviderKey         nvarchar(450) not null,
    ProviderDisplayName nvarchar(max),
    UserId              nvarchar(450) not null
        constraint FK_AspNetUserLogins_AspNetUsers_UserId
            references AspNetUsers
            on delete cascade,
    constraint PK_AspNetUserLogins
        primary key (LoginProvider, ProviderKey)
)
    go

create index IX_AspNetUserLogins_UserId
    on AspNetUserLogins (UserId)
    go

create table AspNetUserRoles
(
    UserId nvarchar(450) not null
        constraint FK_AspNetUserRoles_AspNetUsers_UserId
            references AspNetUsers
            on delete cascade,
    RoleId nvarchar(450) not null
        constraint FK_AspNetUserRoles_AspNetRoles_RoleId
            references AspNetRoles
            on delete cascade,
    constraint PK_AspNetUserRoles
        primary key (UserId, RoleId)
)
    go

create index IX_AspNetUserRoles_RoleId
    on AspNetUserRoles (RoleId)
    go

create table AspNetUserTokens
(
    UserId        nvarchar(450) not null
        constraint FK_AspNetUserTokens_AspNetUsers_UserId
            references AspNetUsers
            on delete cascade,
    LoginProvider nvarchar(450) not null,
    Name          nvarchar(450) not null,
    Value         nvarchar(max),
    constraint PK_AspNetUserTokens
        primary key (UserId, LoginProvider, Name)
)
    go

create index EmailIndex
    on AspNetUsers (NormalizedEmail)
    go

create unique index UserNameIndex
    on AspNetUsers (NormalizedUserName)
    where [NormalizedUserName] IS NOT NULL
go

create table HinhThucDenHans
(
    MaHinhThucDenHan  int identity
        constraint PK_HinhThucDenHans
            primary key,
    TenHinhThucDenHan nvarchar(50) not null
)
    go

create table LoaiGiaoDichs
(
    MaLoaiGiaoDich  int identity
        constraint PK_LoaiGiaoDichs
            primary key,
    TenLoaiGiaoDich nvarchar(100) not null
)
    go

create table LoaiSoTietKiems
(
    MaLoaiSo            int identity
        constraint PK_LoaiSoTietKiems
            primary key,
    TenLoaiSo           nvarchar(max)  not null,
    LaiSuat             float          not null,
    KyHan               int            not null,
    ThoiGianGuiToiThieu int            not null,
    SoTienGuiToiThieu   decimal(18, 2) not null
)
    go

create table BaoCaoNgays
(
    MaBaoCaoNgay  int identity
        constraint PK_BaoCaoNgays
            primary key,
    MaLoaiSo      int            not null
        constraint FK_BaoCaoNgays_LoaiSoTietKiems_MaLoaiSo
            references LoaiSoTietKiems
            on delete cascade,
    Ngay          datetime2      not null,
    TongTienGui   decimal(18, 2) not null,
    TongTienRut   decimal(18, 2) not null,
    NgayTaoBaoCao datetime2      not null
)
    go

create index IX_BaoCaoNgays_MaLoaiSo
    on BaoCaoNgays (MaLoaiSo)
    go

create table BaoCaoThangs
(
    MaBaoCaoThang int identity
        constraint PK_BaoCaoThangs
            primary key,
    MaLoaiSo      int            not null
        constraint FK_BaoCaoThangs_LoaiSoTietKiems_MaLoaiSo
            references LoaiSoTietKiems
            on delete cascade,
    Thang         int            not null,
    Nam           int            not null,
    SoLuongDong   int            not null,
    TongSoTienGui decimal(18, 2) not null,
    TongSoTienRut decimal(18, 2) not null,
    NgayTaoBaoCao datetime2      not null,
    ChenhLech     decimal(18, 2) not null,
    SoLuongMo     int default 0  not null
)
    go

create index IX_BaoCaoThangs_MaLoaiSo
    on BaoCaoThangs (MaLoaiSo)
    go

create table SoTietKiems
(
    MaSoTietKiem     int identity
        constraint PK_SoTietKiems
            primary key,
    Code             nvarchar(max)                                   not null,
    MaLoaiSo         int                                             not null
        constraint FK_SoTietKiems_LoaiSoTietKiems_MaLoaiSo
            references LoaiSoTietKiems
            on delete cascade,
    SoDuSoTietKiem   decimal(18, 2)                                  not null,
    SoTienGui        decimal(18, 2)                                  not null,
    LaiSuatKyHan     decimal(18, 3)                                  not null,
    TrangThai        bit                                             not null,
    LaiSuatApDung    decimal(18, 3)                                  not null,
    NgayMoSo         datetime2                                       not null,
    NgayDongSo       datetime2,
    UserId           nvarchar(450)                                   not null
        constraint FK_SoTietKiems_AspNetUsers_UserId
            references AspNetUsers
            on delete cascade,
    MaHinhThucDenHan int       default 0                             not null
        constraint FK_SoTietKiems_HinhThucDenHans_MaHinhThucDenHan
            references HinhThucDenHans
            on delete cascade,
    NgayDaoHan       datetime2 default '0001-01-01T00:00:00.0000000' not null
)
    go

create table GiaoDichs
(
    MaGiaoDich     int identity
        constraint PK_GiaoDichs
            primary key,
    MaSoTietKiem   int       not null
        constraint FK_GiaoDichs_SoTietKiems_MaSoTietKiem
            references SoTietKiems
            on delete cascade,
    MaLoaiGiaoDich int       not null
        constraint FK_GiaoDichs_LoaiGiaoDichs_MaLoaiGiaoDich
            references LoaiGiaoDichs
            on delete cascade,
    NgayGiaoDich   datetime2 not null,
    SoTien         real      not null
)
    go

create index IX_GiaoDichs_MaLoaiGiaoDich
    on GiaoDichs (MaLoaiGiaoDich)
    go

create index IX_GiaoDichs_MaSoTietKiem
    on GiaoDichs (MaSoTietKiem)
    go

create index IX_SoTietKiems_MaLoaiSo
    on SoTietKiems (MaLoaiSo)
    go

create index IX_SoTietKiems_UserId
    on SoTietKiems (UserId)
    go

create index IX_SoTietKiems_MaHinhThucDenHan
    on SoTietKiems (MaHinhThucDenHan)
    go

create table __EFMigrationsHistory
(
    MigrationId    nvarchar(150) not null
        constraint PK___EFMigrationsHistory
            primary key,
    ProductVersion nvarchar(32)  not null
)
    go

create table sysdiagrams
(
    name         sysname not null,
    principal_id int     not null,
    diagram_id   int identity
        primary key,
    version      int,
    definition   varbinary(max),
    constraint UK_principal_name
        unique (principal_id, name)
)
    go

exec sp_addextendedproperty 'microsoft_database_tools_support', 1, 'SCHEMA', 'dbo', 'TABLE', 'sysdiagrams'
go

