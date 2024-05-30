using DiseaseApp.Data.Concrete.EfCore;
using DiseaseApp.Entity;
using Microsoft.EntityFrameworkCore;

namespace DiseaseApp.Concrete.EfCore
{
    public static class SeedData
    {
        public static void FillTestData(IApplicationBuilder app){
            var context = app.ApplicationServices.CreateScope().ServiceProvider.GetService<DiseaseContext>();

            if (context != null){
                if(context.Database.GetPendingMigrations().Any()){
                    context.Database.Migrate();
                }
                //Test tags
                if(!context.Tags.Any()){
                    context.Tags.AddRange(
                        new Entity.Tag() { Text = "Axiety",Url = "anxiety",Color = Colors.danger},
                        new Entity.Tag() { Text = "Trauma and Stress-related",Url = "trauma-and-stress-related",Color = Colors.warning},
                        new Entity.Tag() { Text = "Eating and Somatoform",Url = "eating-and-somatoform",Color = Colors.info},
                        new Entity.Tag() { Text = "Mood",Url = "mood",Color = Colors.light},
                        new Entity.Tag() { Text = "Personality",Url = "personality",Color = Colors.dark},
                        new Entity.Tag() { Text = "Psychotic",Url = "psychotic",Color = Colors.secondary},
                        new Entity.Tag() { Text = "Other",Url = "Other",Color = Colors.primary}
                        
                    );
                    context.SaveChanges();
                }
                //Test users
                /*
                admin@psyassistant.com : Adm1n!23@
                test@test.com : Test!23@
                muratkuzucu@psyassistant.com : Murat!23@
                */

                if(!context.Users.Any()){
                    context.Users.AddRange(
                        new Entity.User() { UserName = "admin",Name="Administrator",Email="admin@psyassistant.com",
                        Password="c31f1bd624bc69d32ca73ecc46223c85b4d0b26d63a6afc18ad0f26f81277a2f0731a146d322d4496926233b3998acf18058adc55ec19e31308004b9dd7e0e0e",
                        UserRole = "Admin", IsActive = true},
                        new Entity.User() { UserName = "user",Name="User",Email="test@test.com",
                        Password="b850fc3f5734d5e017ae8a2dc3e14ad5185af98686a98d9e4dccb312f97a0b6c6e6ee62ebb4e57acf07b4b627f2ce661ae713c342af60154efe258e7211e41b3"
                        ,UserRole = "User", IsActive = true},
                        new Entity.User() { UserName = "muratkuzucu",Name="Murat",Email="muratkuzucu@psyassistant.com",
                        Password="663b183e9cfc406837bccbb4eefbe9270459360e21866a24886ddc0dfa7024053c86958e911c8e4d21a01ab8a5b037cdc1b5fcd114f443d080db3312cb9b63fb"
                        ,UserRole = "Researcher", IsActive = true}
                    );

                    context.SaveChanges();
                }

               
                        
                        
                        
                    
                
            }
            
        }
        
    }
    
}
