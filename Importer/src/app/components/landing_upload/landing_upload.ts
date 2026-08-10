import { Component, inject, signal } from '@angular/core';
import { TransactionService } from '../../services/transaction.service';
import { ITransaction } from '../../models/transaction.model';
import { EstablishedLinkService } from '../../services/established_link.service';
import { IEstablishedLink } from '../../models/established_link.model';
import { SpendingBucketService } from '../../services/spending_bucket.service';
import { ISpendingBucket } from '../../models/spending_bucket.model';
import * as Papa from 'papaparse';
import { filter, take } from 'rxjs';
import { parseDate } from '../../utils/date.utils';

@Component({
  selector: 'app-root',
  templateUrl: './landing_upload.html',
  standalone: false,
  styleUrl: './landing_upload.css'
})
export class Landing_Upload {
  private transactionService = inject(TransactionService);
  private establishedLinkService = inject(EstablishedLinkService);
  private spendingBucketService = inject(SpendingBucketService);

  constructor() { }

  displayedColumns: string[] = ['EstablishLink', 'Date', 'Card', 'Description', 'Amount', 'Category', 'Reference', 'Bucket', 'Message', 'Remove'];

  trackBy = (index: number, tX: ITransaction) => tX.transactionDate;
  linksList$ = this.establishedLinkService.LinkList$;
  bucketList$ = this.spendingBucketService.BucketList$;

  imports = signal<ITransaction[]>([]);
  newLink = signal<Partial<IEstablishedLink>>({ category: '', containsText: '', bucketId: undefined });

  links: IEstablishedLink[] = [];
  buckets: ISpendingBucket[] = [];

  successMessage: string = '';
  returnMessage: string = '';

  ngOnInit(): void {
    this.linksList$.pipe(
      filter(links => links.length > 0),
    ).subscribe(links => this.links = links);

    this.bucketList$.pipe(
      filter(buckets => buckets.length > 0),
      take(1)
    ).subscribe(buckets => this.buckets = buckets);
  }

  addEstablishedLink(): void {
    if (!this.newLink().bucketId) return;

    let fullLink = this.newLink() as IEstablishedLink;
    this.establishedLinkService.submitLink(fullLink).subscribe((resp) => { 
      if (resp.successCount == 1) {
        this.applyNewLink(fullLink);
        this.newLink.set({ category: '', containsText: '', bucketId: undefined });
        this.establishedLinkService.refreshObservables();
      }
    });
  }

  applyNewLink(link: IEstablishedLink): void {
    this.imports.set(
      this.imports().map(el => {
        if (
          (link.containsText && el.description.toLowerCase().includes(link.containsText.toLowerCase()))
          ||
          (link.category && el.category && el.category.toLowerCase() === link.category.toLowerCase())
        ) {
          return { ...el, bucketId: link.bucketId };
        }
        return el;
      })
    );
  }

  onFileSelected(event: Event) {
    const target = event.currentTarget;
    if (target && target instanceof HTMLInputElement && target.files != null) {
      const file: File = target.files[0];

      if (file) {
        Papa.parse(file, {
          header: true,
          skipEmptyLines: true,
          complete: (result) => {
            let tFunc: (row: any) => ITransaction;
            const headers: string[] = result.meta.fields ?? [];
            const format = this.detectFormat(headers);

            switch (format) {
              case "ING":
                tFunc = this.processING
                break;
              case "BUNQ":
                tFunc = this.processBUNQ
                break;
              case "CHASE":
                tFunc = this.processCHASE
                break;
              case "CAPITALONE":
                tFunc = this.processCAPITALONE
                break;
              default:
                tFunc = this.processING
            }

            this.imports.set(this.parseImports(result.data, tFunc));
          },
          error: (error) => {
            console.error('CSV parse error:', error);
          }
        });
      }
    }
  }

  private parseImports(data: any[], tFunc: (row: any) => ITransaction): ITransaction[] {
    const seen = new Set<string>();

    return data.map(row => {
      const tX = tFunc(row);

      const fingerprint = `${tX.transactionDate}|${tX.card}|${tX.description.toLowerCase()}|${tX.debit}|${tX.amount}|${tX.reference?.toLowerCase()}`;
      if (seen.has(fingerprint))
        tX.message = 'Duplicate?'
      seen.add(fingerprint);

      return tX;
    });
  }

  removeImport(index: number): void {
    this.imports.set(this.imports().filter((_, i) => i !== index));
  }

  establishLink(index: number): void {
    var t = this.imports()[index];
    this.newLink.set({ category: t.category, containsText: t.description, bucketId: t.bucketId });
  }

  submitImports(event: Event) {
    if (this.validateSubmissions()) {
      const upload$ = this.transactionService.submitTransactions(this.imports());
      upload$.subscribe((resp) => {
        if (resp.errorMessage) {
          this.successMessage = '';
          this.returnMessage = resp.errorMessage;
        } else {
          this.successMessage = `${resp.successCount} records successfully imported.`
          this.returnMessage = `${resp.failureCount} records returned for review.`

          this.imports.set(resp.failureSet)
        }
      });
    }
  }

  private validateSubmissions() {
    let valid = true;

    this.imports().forEach(tX => {
      const errors: string[] = [];

      if (!tX.transactionDate) errors.push('Date');
      if (!tX.debit && tX.debit !== false) errors.push('Debit');
      if (!tX.amount && tX.amount !== 0) errors.push('Amount');
      if (!tX.description) errors.push('Description');
      if (!tX.bucketId) errors.push('Bucket');
      if (tX.bucketId && !tX.priority) {
        tX.priority = this.buckets.find(b => b.bucketId === tX.bucketId)?.defaultPriority;
      };

      if (errors.length > 0) {
        tX.message = errors.join(', ');
        valid = false;
      } else {
        tX.message = undefined;
      }
    });

    return valid;
  }

  private detectFormat(headers: string[]): string {
    if (headers.includes('Resulting balance')) return 'ING';
    if (headers.includes('Interest Date')) return 'BUNQ';
    if (headers.includes('Check or Slip #')) return 'CHASE';
    if (headers.includes('Card No.')) return 'CAPITALONE';
    return 'GENERIC';
  }

  private getBucketbyEstablishedLink(category: string | null, description: string): IEstablishedLink | undefined {
    let link = this.links.find(l => l.containsText && description.toLowerCase().includes(l.containsText.toLowerCase()));

    if (link === undefined && category != null)
      link = this.links.find(l => l.category && category.toLowerCase() == l.category);

    return link;
  }

  private getPriorityfromBucket(bID: number | undefined): number | undefined {
    let pri: number | undefined;
    if (bID !== undefined) {
      pri = this.buckets.find(b => b.bucketId == bID)?.defaultPriority;
    }
    return pri;
  }

  private processING = (row: any): ITransaction => {

    const TransactionDate: Date = parseDate(row['Date']);
    const Notifications: string = row['Notifications'];
    const Card = Notifications.indexOf('Card no:') == 0 ? 'ING' + Notifications.substring(24, 28) : 'ING';
    const Description: string = row['Name / Description'];
    const Debit: boolean = row['Debit/credit'].toLowerCase() === 'debit';
    const Amount: number = +(row['Amount (EUR)'].replace('.', '').replace(',', '.'));
    const Category: string = row['Transaction type'];
    const Reference: string = row['Tag'];

    const link = this.getBucketbyEstablishedLink(Category, Description);
    const BucketId = link?.bucketId
    const Priority = this.getPriorityfromBucket(BucketId)

    let tX = this.buildTransaction(TransactionDate, Card, Description, Category, Debit, Amount, Reference, BucketId, Priority, 'EUR');

    return tX;
  }

  private processBUNQ = (row: any): ITransaction => {
    const TransactionDate: Date = parseDate(row['Date'])
    const Description: string = (row['Name'] + ' - ' + row['Description']);
    const Debit: boolean = (row['Amount']).indexOf('-') > -1;
    const Amount: number = Math.abs(+(row['Amount'].replace(',', '')));
    const link = this.getBucketbyEstablishedLink(null, Description);
    const BucketId = link?.bucketId
    const Priority = this.getPriorityfromBucket(BucketId)

    let tX = this.buildTransaction(TransactionDate, 'BUNQ', Description, undefined, Debit, Amount, undefined, BucketId, Priority, 'EUR');

    return tX;
  }

  private processCHASE = (row: any): ITransaction => {
    const TransactionDate: Date = parseDate(row['Posting Date']);
    const Description: string = row['Description'];
    const Debit: boolean = (row['Amount']).indexOf('-') > -1;
    const Amount: number = Math.abs(+(row['Amount']));
    const Reference: string = row['Check or Slip #'];

    const link = this.getBucketbyEstablishedLink(null, Description);
    const BucketId = link?.bucketId
    const Priority = this.getPriorityfromBucket(BucketId)

    let tX = this.buildTransaction(TransactionDate, 'CHASE', Description, undefined, Debit, Amount, Reference, BucketId, Priority, 'USD');

    return tX;
  }

  private processCAPITALONE = (row: any): ITransaction => {
    const TransactionDate: Date = parseDate(row['Posted Date']);
    const Card = 'CO' + row['Card No.'];
    const Description: string = row['Description'];
    const Category: string = row['Category'];
    const Debit: boolean = row['Debit'] != '';
    const Amount: number = Debit ? Math.abs(+(row['Debit'])) : Math.abs(+(row['Credit']))

    const link = this.getBucketbyEstablishedLink(null, Description);
    const BucketId = link?.bucketId
    const Priority = this.getPriorityfromBucket(BucketId)

    let tX = this.buildTransaction(TransactionDate, Card, Description, Category, Debit, Amount, undefined, BucketId, Priority, 'USD');

    return tX;
  }

  private buildTransaction(td: Date, cd: string, de: string, ct: string | undefined, db: boolean,
    am: number, rf: string | undefined, bk: number | undefined, pr: number | undefined, cy: string): ITransaction {

    return {
      transactionDate: td.toISOString().split('T')[0],
      card: cd,
      description: de.substring(0, 150),
      category: ct?.substring(0, 50),
      debit: db,
      amount: am,
      reference: rf?.substring(0, 25),
      bucketId: bk,
      priority: pr,
      currency: cy
    };
  }

}
