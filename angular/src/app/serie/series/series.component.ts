import { Component } from '@angular/core';
import { SerieDto, SerieService } from '@proxy/series';
@Component({
  selector: 'app-series',
  templateUrl: './series.component.html',
  styleUrl: './series.component.scss'
})
export class SeriesComponent {
  series = [] as SerieDto[];
  serieTitle: string = "";
  searchPerformed: boolean = false;
  lastSearchedTitle: string = ''; 
  constructor(private serieService: SerieService) {
  }
  public searchSeries() {
    const trimmedTitle = this.serieTitle.trim();
    
    if (trimmedTitle) {
      this.serieService.search(trimmedTitle, "").subscribe({
        next: (response) => {
          this.series = response || [];
          this.searchPerformed = true;
          this.lastSearchedTitle = trimmedTitle; 
        },
        error: (error) => {
          console.error("Error al buscar series:", error);
          this.series = [];
          this.searchPerformed = true; 
          this.lastSearchedTitle = trimmedTitle;
        },
      });
    }
  }
}
