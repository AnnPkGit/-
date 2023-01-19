export interface PostModel {
  id: string;
  text: string;
  likesCount: number;
  commentsCount: number;
  date: string;
  authorName: string;
  authorAvatar : string;
  images: string[];
}
