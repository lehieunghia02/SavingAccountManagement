using System.Data.Common;

namespace QuanLySoTietKiem.Constaints
{
    public static class MessageConstants
    {
        public static class Success
        {
            public const string CreateAccount = "Tạo tài khoản thành công";
            public const string UpdateAvatar = "Cập nhật ảnh đại diện thành công";
            public const string WithdrawMoney = "Rút tiền thành công";
            public const string AddMoney = "Nạp tiền thành công";
        }
        public static class Error
        {
            public const string InsufficientBalance = "Số dư không đủ";
            public const string AccountNotFound = "Không tìm thấy tài khoản";
            public const string InvalidAmount = "Số tiền không hợp lệ";
        }
        public static class Warning
        {
            public const string WithdrawBeforeMaturity = "Rút tiền trước hạn sẽ được áp dụng lãi suất thấp hơn";
            public const string NotDueDate = "Chưa tới ngày đáo hạn để nạp thêm tiền";
        }

        public static class Login
        {
            public const string AccountNotFound = "Tài khoản không tồn tại";
            public const string EmailNotConfirmed = "Email chưa được xác thực vui lòng kiểm tra email";
            public const string AccountLocked = "Tài khoản đã bị khóa do nhiều lần đăng nhập sai";
            public const string InvalidPassword = "Sai mật khẩu";
            public const string LoginSuccess = "Đăng nhập thành công";
            public const string LoginFailed = "Đăng nhập thất bại";
            public const string AccountLockedOut = "Tài khoản đã bị khóa do nhiều lần đăng nhập sai";
            public const string ErrorDuringPassword = "Có lỗi xảy ra trong quá trình đăng nhập";
        }

        public static class ModelRegister
        {
            //Trường UserName
            public const string UserNameRequired = "Username is required";
            public const string UserNameMinLength = "Username must be at least 3 characters long";
            public const string UserNameMaxLength = "Username must be less than 50 characters long";

            //Trường FullName 
            public const string FullNameRequired = "Họ và tên không được để trống";
            public const string FullNameMinLength = "Họ và tên phải có ít nhất 3 ký tự";
            public const string FullNameMaxLength = "Họ và tên phải nhỏ hơn 50 ký tự";

            //Trường Email
            public const string EmailRequired = "Email không được để trống";
            public const string EmailFormat = "Email chưa đúng định dạng";

            //Trường Password
            public const string PasswordRequired = "Mật khẩu không được để trống";
            public const string PasswordMinLength = "Mật khẩu phải có ít nhất 6 ký tự ";
            public const string PasswordMaxLength = "Mật khẩu phải nhỏ hơn 30 ký tự";
            
            //Trường Address 
            public const string AddressRequired = "Địa chỉ không được để trống";
            public const string AddressMinLength = "Đại chỉ phải có ít nhất 3 ký tự";
            public const string AddressMaxLength = "Địa chỉ phải nhỏ hơn 255 ký tự";

            //Trường CCCD (Căn cước công dân)
            public const string CitizenIdentificationRequired = "Căn cước công dân không được để trống"; 
            public const string CitizenIdentificationMinLength = "Căn cước công dân phải có ít nhất 9 ký tự";
            public const string CitizenIdentificationMaxLength = "Căn cước công dân phải nhỏ hơn 12 ký tự";

            //Trường xác nhận mật khẩu
            public const string ConfirmPasswordRequired = "Xác nhận mật khẩu không được để trống";
            public const string ConfirmPasswordMinLength = "Xác nhận mật khẩu phải có ít nhất 6 ký tự";
            public const string ConfirmPasswordMaxLength = "Xác nhận mật khẩu phải nhỏ hơn 30 ký tự";
            public const string ComparePassword = "Mật khẩu và xác nhận mật khẩu không khớp";

            //Trường số điện thoại
            public const string PhoneNumberRequired = "Số điện thoại không được để trống";
            public const string PhoneNumberMinLength = "Số điện thoại phải có ít nhất 10 ký tự";
            public const string PhoneNumberMaxLength = "Số điện thoại phải nhỏ hơn 11 ký tự";

            //Trường số dư tài khoản
            public const int AccountBalance = 100000000;
        }

        public static class EditProfileModel
        {
            public const string FullNameRequired = "Vui lòng nhập họ tên";
            public const string FullNameMaxStringLength = "Họ tên không được vượt quá 50 ký tự";

            public const string EmailRequired = "Vui lòng nhập địa chỉ email"; 
            public const string EmailFormat = "Email chưa đúng định dạng";


            //Trường địa chỉ 
            public const string AddressRequired = "Vui lòng nhập địa chỉ";
            public const string AddressMinLength = "Địa chỉ phải có ít nhất 3 ký tự";
            public const string AddressMaxLength = "Địa chỉ phải nhỏ hơn 255 ký tự";


            //Trường số điện thoại 
            public const string PhoneNumberRequired = "Vui lòng nhập số điện thoại";
            public const string PhoneNumberMinLength = "Số điện thoại phải có ít nhất 10 ký tự";
            public const string PhoneNumberMaxLength = "Số điện thoại phải nhỏ hơn 11 ký tự";
        }
    }
}
