// Corespunde PostReadDto din backend
export interface Post {
  id: number;
  descriere: string;
  imageUrl: string;
  dataPublicarii: string;
  numeAutor: string;
}

export interface CreatePostDto {
  descriere: string;
  imageUrl: string;
}

export interface UpdatePostDto {
  descriere?: string;
  imageUrl?: string;
}

// Corespunde CommentReadDto din backend
export interface Comment {
  id: number;
  content: string;
  date: string;
  username: string;
}

export interface CreateCommentDto {
  content: string;
  postId: number;
}

// Corespunde LikeReadDto din backend
export interface Like {
  id: number;
  postId: number;
  username: string;
}

export interface CreateLikeDto {
  postId: number;
}

// Corespunde UserReadDto din backend
export interface User {
  id: string;
  username: string;
  email: string;
}

// Corespunde UpdateUserDto din backend (toate campurile required)
export interface UpdateUserDto {
  username: string;
  email: string;
  nume: string;
  prenume: string;
}
