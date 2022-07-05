using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.IO;

namespace OperatingSystemsPractices.Source.Vk
{
    public class Post
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string[] Photos { get; set; }
        public string[] Hrefs { get; set; }

        public static void CreateJsonFile(string filePath, List<Post> posts)
        {
            File.WriteAllText(filePath, JsonSerializer.Serialize(posts, Json.GetOptions()));
        }

        public static void AddInJsonFile(string filePath, List<Post> posts, bool checkExists = true)            
        {
            if (!File.Exists(filePath))
            {
                CreateJsonFile(filePath, posts);
                return;
            }

            List<Post> existsPosts = JsonSerializer.Deserialize<List<Post>>(File.ReadAllText(filePath), Json.GetOptions());
            if (existsPosts == null || !existsPosts.Any())
            {
                CreateJsonFile(filePath, posts);
                return;
            }

            List<Post> newPosts;
            if (checkExists)
                newPosts = posts.Except(existsPosts).ToList();
            else
                newPosts = posts;

            if (newPosts.Any())
            {
                FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite);
                StreamWriter writer = new StreamWriter(stream);

                stream.Seek(-3, SeekOrigin.End);
                writer.Write(',' + JsonSerializer.Serialize(newPosts, Json.GetOptions()).Substring(1));
                writer.Close();
                stream.Close();
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is Post post) return Id == post.Id;
            return false;
        }

        public override int GetHashCode() => Id.GetHashCode();
    }
}
