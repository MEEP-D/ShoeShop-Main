using Microsoft.AspNetCore.Identity;
using ShoeShop.Models;

namespace ShoeShop.Data.Seeder
{
    public class UserSeeder
    {
        public UserSeeder(IApplicationBuilder applicationBuilder) {
            UsersAsync(applicationBuilder).Wait();
        }
        public async Task UsersAsync(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                var adminUser = await userManager.FindByEmailAsync("transinh085@gmail.com");
                if (adminUser == null)
                {
                    var address = new List<Address>
                    {
                        new Address
                        {
                            FullName = "Nguyen Van A",
                            Phone = "0374418254",
                            Email = "abc@gmail.com",
                            SpecificAddress = "Bac Tu Liem,Ha Noi, Ha Noi",
                            IsDefault = true
                        },
                        new Address
                        {
                            FullName = "Nguyen Van B",
                            Phone = "0374418253",
                            Email = "123@gmail.com",
                            SpecificAddress = "An Duc, Hoai Duc, Hoai Duc"
                        }
                    };
                    var newAdminUser = new AppUser()
                    {
                        FullName = "Trần Nhật Sinh",
                        UserName = "transinh085",
                        Email = "transinh085@gmail.com",
                        EmailConfirmed = true,
                        PhoneNumber = "0123456789",
                        ProfileImageUrl = "https://avatars.githubusercontent.com/u/45101901?v=4",
                        Status = true,
                        Gender = 0,
                        BirthDay = DateTime.Now,
                        Addresses = address,
                    };
                    await userManager.CreateAsync(newAdminUser, "Coding@1234?");
                    await userManager.AddToRoleAsync(newAdminUser, UserRoles.Admin);
                }

                var appUser = await userManager.FindByEmailAsync("musicanime2501@gmail.com");
                if (appUser == null)
                {
					var address = new List<Address>
					{
						new Address
						{
							FullName = "Nguyen Van A",
							Phone = "03879133472",
							Email = "duong@gmail.com",
							SpecificAddress = "Tan Binh",
							IsDefault = true
						},
						new Address
						{
							FullName = "Nguyen Van B",
							Phone = "0369765342",
							Email = "ducduong@gmail.com",
							SpecificAddress = "An Duc, Hoai An, Binh Dinh"
						}
					};
					var newAppUser = new AppUser()
                    {
                        FullName = "Trịnh Đức Dương",
                        UserName = "Duong",
                        Email = "musicanime2607@gmail.com",
                        EmailConfirmed = true,
                        PhoneNumber = "0123456789",
                        ProfileImageUrl = "",
                        Status = true,
                        Gender = 0,
                        BirthDay = DateTime.Now,
						Addresses = address,
					};
                    await userManager.CreateAsync(newAppUser, "Coding@1234?");
                    await userManager.AddToRoleAsync(newAppUser, UserRoles.Admin);
                }

                var appUser1 = await userManager.FindByEmailAsync("acduong4567@gmail.com");
                if (appUser == null)
                {
                    var newAppUser = new AppUser()
                    {
                        FullName = "Đức Dương",
                        UserName = "Duong123",
                        Email = "trinhducduong267@gmail.com",
                        EmailConfirmed = true,
                        PhoneNumber = "0123456789",
                        ProfileImageUrl = "",
                        Status = true,
                        Gender = 0,
                        BirthDay = DateTime.Now,
					};
                    await userManager.CreateAsync(newAppUser, "Coding@1234?");
                    await userManager.AddToRoleAsync(newAppUser, UserRoles.Admin);
                }
            }
        }
    }
}
