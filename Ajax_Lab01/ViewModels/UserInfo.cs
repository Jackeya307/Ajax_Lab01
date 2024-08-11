namespace Ajax_Lab01.ViewModels {
    public class UserInfo {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public int? Age { get; set; }

        public IFormFile[]? UserPhoto { get; set; }
    }
}
