-- Create store procedure to get all users
CREATE PROCEDURE sp_GetAllLoaiSoTietKiem
AS
BEGIN
    SELECT * FROM LoaiSoTietKiems
END

