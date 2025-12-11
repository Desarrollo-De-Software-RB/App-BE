import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { RatingService } from '../../proxy/series/rating.service';
import { RatingDto, CreateUpdateRatingDto } from '../../proxy/series/models';
import { ToasterService } from '@abp/ng.theme.shared';
import { AuthService, ConfigStateService } from '@abp/ng.core';

@Component({
  selector: 'app-rating',
  templateUrl: './rating.component.html',
  styleUrls: ['./rating.component.scss']
})
export class RatingComponent implements OnInit {
  @Input() serieId!: number;
  @Output() ratingUpdated = new EventEmitter<void>();
  ratings: RatingDto[] = [];
  newRating: CreateUpdateRatingDto = { serieId: 0, score: 0, comment: '' };
  hoverScore = 0;
  isLoggedIn = false;
  currentUserId: string | null = null;

  constructor(
    private ratingService: RatingService,
    private toaster: ToasterService,
    private authService: AuthService,
    private configState: ConfigStateService
  ) { }

  ngOnInit(): void {
    this.isLoggedIn = this.authService.isAuthenticated;
    if (this.isLoggedIn) {
      const currentUser = this.configState.getOne('currentUser');
      this.currentUserId = currentUser?.id;
    }

    if (this.serieId) {
      this.loadRatings();
    }
  }

  userRating: RatingDto | undefined;

  loadRatings() {
    this.ratingService.getSeriesRatings(this.serieId).subscribe(result => {
      console.log('Ratings loaded:', result);

      if (this.currentUserId) {
        this.userRating = result.find(r => r.userId === this.currentUserId);
        this.ratings = result.filter(r => r.userId !== this.currentUserId);
      } else {
        this.userRating = undefined;
        this.ratings = result;
      }
    });
  }

  setScore(score: number) {
    this.newRating.score = score;
  }

  editRating(rating: RatingDto) {
    if (this.currentUserId && rating.userId === this.currentUserId) {
      this.newRating = {
        serieId: rating.serieId,
        score: rating.score,
        comment: rating.comment
      };
      // Scroll to form
      document.querySelector('.rating-form')?.scrollIntoView({ behavior: 'smooth' });
    }
  }

  submitRating() {
    if (this.newRating.score === 0) {
      this.toaster.warn('Please select a star rating.');
      return;
    }

    this.newRating.serieId = this.serieId;

    this.ratingService.rateSeries(this.newRating).subscribe(() => {
      this.toaster.success('Rating submitted successfully');
      this.newRating = { serieId: 0, score: 0, comment: '' };
      this.loadRatings();
      this.ratingUpdated.emit();
    });
  }

  expandedRatings: Set<number> = new Set();
  visibleRatingsCount = 4;

  toggleExpansion(id: number) {
    if (this.expandedRatings.has(id)) {
      this.expandedRatings.delete(id);
    } else {
      this.expandedRatings.add(id);
    }
  }

  isExpanded(id: number): boolean {
    return this.expandedRatings.has(id);
  }

  showMoreRatings() {
    this.visibleRatingsCount += 4;
  }
}
