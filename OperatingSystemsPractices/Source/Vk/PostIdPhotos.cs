using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.IO;

namespace OperatingSystemsPractices.Source.Vk
{
    public class PostIdPhotos
    {
        public string Id { get; set; }

        public string[] Photos { get; set; }

        public static List<PostIdPhotos> Create(List<Post> posts)
        {
            List<PostIdPhotos> postsIdPhotos = new List<PostIdPhotos>();
            foreach (var post in posts)
            {
                postsIdPhotos.Add(new PostIdPhotos() { Id = post.Id, Photos = post.Photos });
            }
            return postsIdPhotos.Where(obj => obj.Photos != Array.Empty<string>()).ToList();
        }

        public static void CreateJsonFile(string filePath, List<PostIdPhotos> posts)
        {
            File.WriteAllText(filePath, JsonSerializer.Serialize(posts, Json.GetOptions()));
        }

        public static void AddInJsonFile(string filePath, List<PostIdPhotos> posts, bool checkExists = true)
        {
            if (!File.Exists(filePath))
            {
                CreateJsonFile(filePath, posts);
                return;
            }

            List<PostIdPhotos> existsPosts = JsonSerializer.Deserialize<List<PostIdPhotos>>(File.ReadAllText(filePath), Json.GetOptions());
            if (existsPosts == null || !existsPosts.Any())
            {
                CreateJsonFile(filePath, posts);
                return;
            }

            List<PostIdPhotos> newPosts;
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

        public static List<PostIdPhotos> ReadFromJsonFile(string filePath)
        {
            return JsonSerializer.Deserialize<List<PostIdPhotos>>(File.ReadAllText(filePath), Json.GetOptions());
        }

        public override bool Equals(object obj)
        {
            if (obj is PostIdPhotos postIdPhotos) return Id == postIdPhotos.Id;
            return false;
        }

        public override int GetHashCode() => Id.GetHashCode();
    }
}
