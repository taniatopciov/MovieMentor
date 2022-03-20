import Director from "./director";
import Actor from "./actor";

export default interface Movie {
  id: number;
  title: string;
  genres: string[];
  director: Director[];
  actor: Actor[];
  year: number;
  awards: string[];
  duration: number;
  country: string;
  rating: string;
}
