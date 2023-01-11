import { Component } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})

export class HomeComponent {
  public posts: Post[] = [];

  constructor() {
    this.posts = [
      { text: "1 text" },
      {
        text: `Romeo and Juliet is a tragedy written by William Shakespeare early in his career about the romance between two Italian youths from feuding families.
        It was among Shakespeares most popular plays during his lifetime and, along with Hamlet, is one of his most frequently performed plays.
        Today, the title characters are regarded as archetypal young lovers.
        Romeo and Juliet belongs to a tradition of tragic romances stretching back to antiquity.The plot is based on an Italian tale translated into verse
        as The Tragical History of Romeus and Juliet by Arthur Brooke in 1562 and retold in prose in Palace of Pleasure by William Painter in 1567. Shakespeare borrowed
        heavily from both but expanded the plot by developing a number of supporting characters, particularly Mercutio and Paris.Believed to have been written between
        1591 and 1595, the play was first published in a quarto version in 1597.
        The text of the first quarto version was of poor quality, however, and later editions corrected the text to conform more closely with Shakespeares original.` },
      { text: "2 text" },
      {
        text: `Romeo and Juliet is a tragedy written by William Shakespeare early in his career about the romance between two Italian youths from feuding families.
        It was among Shakespeares most popular plays during his lifetime and, along with Hamlet, is one of his most frequently performed plays.
        Today, the title characters are regarded as archetypal young lovers.
        Romeo and Juliet belongs to a tradition of tragic romances stretching back to antiquity.The plot is based on an Italian tale translated into verse
        as The Tragical History of Romeus and Juliet by Arthur Brooke in 1562 and retold in prose in Palace of Pleasure by William Painter in 1567. Shakespeare borrowed
        heavily from both but expanded the plot by developing a number of supporting characters, particularly Mercutio and Paris.Believed to have been written between
        1591 and 1595, the play was first published in a quarto version in 1597.
        The text of the first quarto version was of poor quality, however, and later editions corrected the text to conform more closely with Shakespeares original.` },
      {
        text: `Romeo and Juliet is a tragedy written by William Shakespeare early in his career about the romance between two Italian youths from feuding families.
        It was among Shakespeares most popular plays during his lifetime and, along with Hamlet, is one of his most frequently performed plays.
        Today, the title characters are regarded as archetypal young lovers.
        Romeo and Juliet belongs to a tradition of tragic romances stretching back to antiquity.The plot is based on an Italian tale translated into verse
        as The Tragical History of Romeus and Juliet by Arthur Brooke in 1562 and retold in prose in Palace of Pleasure by William Painter in 1567. Shakespeare borrowed
        heavily from both but expanded the plot by developing a number of supporting characters, particularly Mercutio and Paris.Believed to have been written between
        1591 and 1595, the play was first published in a quarto version in 1597.
        The text of the first quarto version was of poor quality, however, and later editions corrected the text to conform more closely with Shakespeares original.` }
    ];
  }

}

interface Post {
  text: string;
}
