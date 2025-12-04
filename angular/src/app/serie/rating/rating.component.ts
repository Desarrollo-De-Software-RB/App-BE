import { Component, Input, OnInit } from '@angular/core';
import { RatingService } from '../../proxy/series/rating.service';
import { RatingDto, CreateUpdateRatingDto } from '../../proxy/series/models';
import { ToasterService } from '@abp/ng.theme.shared';
import { AuthService } from '@abp/ng.core';

@Component({
  selector: 'app-rating',
  templateUrl: './rating.component.html',
  styleUrls: ['./rating.component.scss']
})
export class RatingComponent implements OnInit {
  @Input() serieId!: number;
  ratings: RatingDto[] = [];
  newRating: CreateUpdateRatingDto = { serieId: 0, score: 0, comment: '' };
  hoverScore = 0;
  isLoggedIn = false;
  currentUserRating: RatingDto | null = null;

  constructor(
    private ratingService: RatingService,
    private toaster: ToasterService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.isLoggedIn = this.authService.isAuthenticated;
    if (this.serieId) {
      this.loadRatings();
    }
  }

  loadRatings() {
    this.ratingService.getList({ serieId: this.serieId }).subscribe(result => {
      this.ratings = result.items;
      this.checkUserRating();
    });
  }

  checkUserRating() {
    // This logic might need adjustment depending on how we identify the current user's rating from the list
    // For now, we'll just display all ratings. 
    // If the backend returns the current user's ID, we could filter.
    // Assuming the backend handles "one rating per user" or we just let them add more.
    // But usually we want to know if the user already rated to show "Edit" or "Your Rating".
  }

  setScore(score: number) {
    this.newRating.score = score;
  }

  submitRating() {
    if (this.newRating.score === 0) {
      this.toaster.warn('Please select a star rating.');
      return;
    }

    this.newRating.serieId = this.serieId;

    this.ratingService.create(this.newRating).subscribe(() => {
      this.toaster.success('Rating submitted successfully');
      this.newRating = { serieId: 0, score: 0, comment: '' };
      this.loadRatings();
    });
  }
}
