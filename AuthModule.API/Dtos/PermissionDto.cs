namespace AuthModule.API.Dtos
{
    public class PermissionDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
            
        public string Description { get; set; }

        public PermissionDto(int id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }

    }
}
