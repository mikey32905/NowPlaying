﻿namespace NowPlaying.Models
{

    public class MovieVideosResponse
    {
        public int Id { get; set; }
        public List<Video> Results { get; set; } = [];
    }

}
