import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PersonalSettingsComponent } from './personal-settings.component';
import { ReactiveFormsModule } from '@angular/forms';

@NgModule({
    declarations: [PersonalSettingsComponent],
    imports: [
        CommonModule,
        ReactiveFormsModule
    ],
    exports: [PersonalSettingsComponent]
})
export class PersonalSettingsModule { }
