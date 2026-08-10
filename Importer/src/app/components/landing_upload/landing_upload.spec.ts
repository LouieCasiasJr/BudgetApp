import { TestBed } from '@angular/core/testing';
import { RouterModule } from '@angular/router';
import { Landing_Upload } from './landing_upload';

describe('landing_upload', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        RouterModule.forRoot([])
      ],
      declarations: [
        Landing_Upload
      ],
    }).compileComponents();
  });

  it('should create the app', () => {
    const fixture = TestBed.createComponent(Landing_Upload);
    const app = fixture.componentInstance;
    expect(app).toBeTruthy();
  });

  it('should render title', async () => {
    const fixture = TestBed.createComponent(Landing_Upload);
    await fixture.whenStable();
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('h1')?.textContent).toContain('Hello, Importer');
  });
});
